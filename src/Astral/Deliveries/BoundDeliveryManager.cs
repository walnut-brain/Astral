using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Payloads;
using Astral.Transport;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Deliveries
{
    public class BoundDeliveryManager<TStore>
        where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
    {
        private readonly string _sponsor;
        private readonly TimeSpan _leaseInterval;
        private readonly CancellationDisposable _dispose;

        private readonly ConcurrentDictionary<Guid, IDisposable> _leases =
            new ConcurrentDictionary<Guid, IDisposable>();

        private readonly IServiceProvider _provider;
        private readonly Task _renewLoop;


        public BoundDeliveryManager(IServiceProvider provider, TimeSpan leaseInterval)
        {
            _leaseInterval = leaseInterval;
            _provider = provider;
            _sponsor = $"{Environment.MachineName}-{Guid.NewGuid()}";
            _dispose = new CancellationDisposable();
            _renewLoop = Loop(_dispose.Token);

        }


        public async Task<Guid> Prepare<T>(TStore store, T message, DeliveryOperation operation, TimeSpan messageTtl, AfterCommitDelivery afterCommit,
            PayloadSender<T> sender, ToPayloadOptions<byte[]> toPayloadOptions, Option<string> key)
        {
            var service = store.DeliveryService;
            using (var work = store.BeginWork())
            {
                key.IfSome(p => service.RemoveByKey(operation.Operation, p));
                var (guid, payload) = await service.NewDelivery(message?.GetType() ?? typeof(T), message, operation,
                    messageTtl, _sponsor,
                    afterCommit is AfterCommitDelivery.NoOpType ? TimeSpan.Zero :_leaseInterval + _leaseInterval);
                switch (afterCommit)
                {
                    case AfterCommitDelivery.NoOpType _:
                        break;
                    case AfterCommitDelivery.SendType send:
                        work.CommitEvents.Subscribe(_ => {}, () => AddDelivery(guid, payload, new Lazy<T>(() => message), send.OnSuccess, sender, toPayloadOptions, true).Wait());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(AfterCommitDelivery)} - {afterCommit}");
                }
                work.Commit();
                return guid;
            }
            
        }




        private async Task AddDelivery<T>(Guid deliveryId, Payload payload, Lazy<T> message, 
            OnDeliverySuccess onSuccess, PayloadSender<T> sender, ToPayloadOptions<byte[]> toPayloadOptions, bool pickup)
        {
            Payload<byte[]> rawPayload;
            switch (payload)
            {
                case Payload<byte[]> pb when Equals(pb.ContentType, toPayloadOptions.ContentType):
                    rawPayload = pb;
                    break;
                case Payload<string> ps when ps.ContentType.Name == toPayloadOptions.ContentType.Name:
                    var encodingName = toPayloadOptions.ContentType.CharSet ?? Encoding.UTF8.WebName;
                    try
                    {
                        var encoding = Encoding.GetEncoding(encodingName);
                        rawPayload = new Payload<byte[]>(ps.TypeCode, toPayloadOptions.ContentType, encoding.GetBytes(ps.Data));
                    }
                    catch
                    {
                        rawPayload = Payload.ToPayload(message.Value, toPayloadOptions).IfFailThrow();
                    }
                    break;
                default:
                    rawPayload = Payload.ToPayload(message.Value, toPayloadOptions).IfFailThrow();
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
                await ContinueDoInLease(sender(message, rawPayload, compositeCancellation.Token), deliveryId, onSuccess);
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
                await srv.TryUpdateLease(deliveryId, _sponsor, _leaseInterval + _leaseInterval, null));

        }

        private async Task ContinueDoInLease(Task task, Guid deliveryId, OnDeliverySuccess behavior)
        {
            try
            {
                await task;
                await DoInScope(async p =>
                {
                    switch (behavior)
                    {
                        case OnDeliverySuccess.ArchiveType archiveType:
                            await p.TryUpdateLease(deliveryId, _sponsor, Timeout.InfiniteTimeSpan,
                                archiveType.DeleteAfter);
                            break;
                        case OnDeliverySuccess.DeleteType deleteType:
                            await p.DeleteDelivery(deliveryId);
                            break;
                        case OnDeliverySuccess.RedeliveryType redeliveryType:
                            await p.TryUpdateLease(deliveryId, _sponsor, redeliveryType.RedeliveryAfter, null);
                            break;
                    }
                });
            }
            catch  
            {
                if (_dispose.IsDisposed)
                    return;
                await DoInScope(async p => await p.TryUpdateLease(deliveryId, _sponsor, TimeSpan.Zero, null));
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
                var provider = scope.ServiceProvider.GetRequiredService<TStore>();
                using (var uow = provider.BeginWork())
                {
                    var dataService = provider.DeliveryService;
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
                var provider = scope.ServiceProvider.GetRequiredService<TStore>();
                using (var uow = provider.BeginWork())
                {
                    var dataService = provider.DeliveryService;
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