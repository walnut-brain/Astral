using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Mime;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Specifications;
using Astral.Transport;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Deliveries
{
    internal class BoundDeliveryManager<TStore>
    {
        private readonly string _sponsor;
        private readonly TimeSpan _leaseInterval;
        private readonly CancellationDisposable _dispose;

        private readonly ConcurrentDictionary<Guid, IDisposable> _leases =
            new ConcurrentDictionary<Guid, IDisposable>();

        private readonly IServiceProvider _provider;
        private readonly Task _renewLoop;
        private ILogger<BoundDeliveryManager<TStore>> _logger;


        public BoundDeliveryManager(IServiceProvider provider, TimeSpan leaseInterval)
        {
            _leaseInterval = leaseInterval;
            _provider = provider;
            _sponsor = $"{Environment.MachineName}-{Guid.NewGuid()}";
            _dispose = new CancellationDisposable();
            _renewLoop = Loop(_dispose.Token);
            _logger = provider.GetService<ILogger<BoundDeliveryManager<TStore>>>();
        }


        public async Task Prepare<T>(
            IUnitOfWork<TStore> store, 
            EndpointConfig endpoint,
            Guid deliveryId,
            T message,
            DeliveryReply reply,
            PayloadSender<T> sender,
            Option<DeliveryOnSuccess>  policy)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            var service = store.GetService<IDeliveryDataService<TStore>>();
            
            var payload =
                await service.NewDelivery(endpoint, deliveryId, reply, message, _sponsor,
                    policy.Map(_ => _leaseInterval + +_leaseInterval).IfNone(TimeSpan.Zero));
            policy.IfSome(plc =>
                store.WorkResult.Subscribe(_ => { }, () => AddDelivery(deliveryId, payload,
                    new Lazy<T>(() => message),
                    plc, sender, endpoint.PayloadEncode, true).Wait()));
        }
        
        private async Task AddDelivery<T>(Guid deliveryId, Payload payload, Lazy<T> message, 
            DeliveryOnSuccess policy, PayloadSender<T> sender, PayloadEncode<byte[]> payloadEncode, bool pickup)
        {
            Payload<byte[]> rawPayload;
            switch (payload)
            {
                case Payload<byte[]> pb when Equals(pb.ContentType, payloadEncode.ContentType):
                    rawPayload = pb;
                    break;
                case Payload<string> ps when ps.ContentType.Name == payloadEncode.ContentType.Name:
                    var encodingName = payloadEncode.ContentType.CharSet ?? Encoding.UTF8.WebName;
                    try
                    {
                        var encoding = Encoding.GetEncoding(encodingName);
                        rawPayload = new Payload<byte[]>(ps.TypeCode, payloadEncode.ContentType, encoding.GetBytes(ps.Data));
                    }
                    catch
                    {
                        rawPayload = Payload.ToPayload(_logger, message.Value, payloadEncode).Unwrap();
                    }
                    break;
                default:
                    rawPayload = Payload.ToPayload(_logger, message.Value, payloadEncode).Unwrap();
                    break;
            }

            if (_dispose.IsDisposed) throw new ObjectDisposedException(nameof(BoundDeliveryManager<TStore>));
            if (pickup && !await PickupLease(deliveryId))
                throw new CannotTakeLeaseException($"Cannot take lease on delivery {deliveryId}");
            var leaseCancellation = new CancellationDisposable();
            if (!_leases.TryAdd(deliveryId, leaseCancellation))
            {
                leaseCancellation.Dispose();
                throw new CannotTakeLeaseException($"Already taked lease on delivery {deliveryId}");
            }

            var compositeCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(leaseCancellation.Token, _dispose.Token);

            try
            {
                await ContinueDoInLease(sender(message, rawPayload, deliveryId.ToString("D"), compositeCancellation.Token), deliveryId, policy);
            }
            finally
            {
                leaseCancellation.Dispose();
                compositeCancellation.Dispose();
            }


        }

        private Task<bool> PickupLease(Guid deliveryId)
        {
            return DoInScope(async srv =>
                await srv.TryPickupLease(deliveryId, _sponsor, _leaseInterval + _leaseInterval));

        }

        private async Task ContinueDoInLease(Task task, Guid deliveryId, DeliveryOnSuccess policy)
        {
            try
            {
                await task;
                await DoInScope(async p =>
                {
                    await policy.Match(() => p.DeleteDelivery(deliveryId),
                        () => p.TryFreeLease(deliveryId, _sponsor, Timeout.InfiniteTimeSpan),
                        ts => p.TryFreeLease(deliveryId, _sponsor, ts));
                    
                });
            }
            catch  
            {
                if (_dispose.IsDisposed)
                    return;
                await DoInScope(async p => await p.TryFreeLease(deliveryId, _sponsor, TimeSpan.Zero));
            }
        }


        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var current = _leases.Keys.ToList();
                var renewed =
                    (await DoInScope(async p => await p.RenewLeases(_sponsor, _leaseInterval + _leaseInterval))).ToList();
                var toRemove = current.Where(p => renewed.All(t => t != p));
                foreach (var guid in toRemove)
                    if (_leases.TryRemove(guid, out var p))
                        p.Dispose();
                await Task.Delay(_leaseInterval, token);
            }
        }


        private Task CleanLeases()
        {
            return DoInScope(async srv => await srv.CleanSponsorLeases(_sponsor));
        }


        private async Task<T> DoInScope<T>(Func<IDeliveryDataService<TStore>, Task<T>> func)
        {
            using (var scope = _provider.CreateScope())
            {
                using (var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork<TStore>>())
                {
                    var dataService = uow.GetService<IDeliveryDataService<TStore>>();
                    var result = await func(dataService);
                    uow.Commit();
                    return result;
                }
            }
        }

        private async Task DoInScope(Func<IDeliveryDataService<TStore>, Task> func)
        {
            using (var scope = _provider.CreateScope())
            {
                using (var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork<TStore>>())
                {
                    var dataService = uow.GetService<IDeliveryDataService<TStore>>();
                    await func(dataService);
                    uow.Commit();
                }
            }
        }



        public void Dispose()
        {
            if (_dispose.IsDisposed) return;
            _dispose.Dispose();
            _renewLoop.Wait();
            CleanLeases().Wait();
        }

    }

    
}