using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.DependencyInjection;
using Astral.Exceptions;
using Astral.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Delivery
{
    public class DeliveryManager<TStore> : IDisposable
        where TStore : IStore<TStore>
    {
        private readonly CancellationDisposable _dispose;
        private readonly TimeSpan _leaseInterval;

        private readonly ConcurrentDictionary<Guid, IDisposable> _leases =
            new ConcurrentDictionary<Guid, IDisposable>();

        private readonly Task _renewLoop;
        private readonly Func<IDedicatedScope> _scopeProvider;
        private readonly string _sponsor;


        public DeliveryManager(Func<IDedicatedScope> scopeProvider, TimeSpan leaseInterval)
        {
            _scopeProvider = scopeProvider;
            _leaseInterval = leaseInterval;
            _sponsor = $"{Environment.MachineName}-{Guid.NewGuid()}";
            _dispose = new CancellationDisposable();
            _renewLoop = Loop(_dispose.Token);
        }


        public void Dispose()
        {
            if (_dispose.IsDisposed) return;
            _dispose.Dispose();
            _renewLoop.Wait();
            CleanLeases().Wait();
        }

        public async Task AddDelivery(Guid deliveryId, Func<CancellationToken, Task> doInLease,
            OnDeliverySuccess behavior)
        {
            if (_dispose.IsDisposed) throw new ObjectDisposedException("DeliveryManager");
            if (!await PickupLease(deliveryId))
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
                await ContinueDoInLease(doInLease(compositeCancellation.Token), deliveryId, behavior);
            }
            finally
            {
                leaseCancellation.Dispose();
                compositeCancellation.Dispose();
            }
            
            
            
        }
        
        
        private async Task ContinueDoInLease(Task task, Guid deliveryId, OnDeliverySuccess behavior)
        {
            try
            {
                await task;
                await DoInScope(p => behavior.Apply(p, deliveryId, _sponsor));
            }
            catch (Exception ex) when (ex.IsCancellation())
            {
                if(_dispose.IsDisposed)
                    return;
                await DoInScope(p => p
                    .Where(t => t.Sponsor == _sponsor && t.DeliveryId == deliveryId)
                    .Set(t => t.LeasedTo, DateTimeOffset.Now)
                    .Set(t => t.Sponsor, (string) null)
                    .Update());
            }
            catch (Exception ex)
            {
                await DoInScope(p => p
                    .Where(t => t.Sponsor == _sponsor && t.DeliveryId == deliveryId)
                    .Set(t => t.LeasedTo, DateTimeOffset.Now)
                    .Set(t => t.Sponsor, (string) null)
                    .Set(t => t.LastError, ex.Message)
                    .Update());
            }
            
        }
        

        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var current = _leases.Keys.ToList();
                var renewed =
                    await DoInScope(async p => await
                        p
                            .Where(t => t.Sponsor == _sponsor)
                            .Set(t => t.LeasedTo, DateTimeOffset.Now + _leaseInterval + _leaseInterval)
                            .UpdatedKeys());
                var toRemove = current.Where(p => renewed.All(t => t != p));
                foreach (var guid in toRemove)
                    if (_leases.TryRemove(guid, out var p))
                        p.Dispose();
                await Task.Delay(_leaseInterval, token);
            }
        }


        private Task<bool> PickupLease(Guid deliveryId)
        {
            return DoInScope(async srv =>
                await srv
                    .Where(p => p.DeliveryId == deliveryId && (p.Sponsor == _sponsor || p.Sponsor == null) &&
                                !p.Delivered && p.Ttl < DateTimeOffset.Now)
                    .Set(p => p.Sponsor, _sponsor)
                    .Set(p => p.LeasedTo, DateTimeOffset.Now + _leaseInterval + _leaseInterval)
                    .Update() == 1);
                        
        }

        private Task CleanLeases()
        {
            return DoInScope(async srv => await srv
                .Where(p => p.Sponsor == _sponsor)
                .Set(p => p.Sponsor, (string) null)
                .Set(p => p.LeasedTo, DateTimeOffset.Now)
                .Update());
        }


        private async Task<T> DoInScope<T>(Func<IDeliveryDataService<TStore>, Task<T>> func)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<TStore>();
                using (var uow = await provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TStore>>();
                    var result = await func(dataService);
                    await uow.Commit();
                    return result;
                }
            }
        }

        private async Task DoInScope(Func<IDeliveryDataService<TStore>, Task> action)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<IStore<TStore>>();
                using (var uow = await provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TStore>>();
                    await action(dataService);
                    await uow.Commit();
                }
            }
        }

        
    }
}