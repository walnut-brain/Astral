using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.DependencyInjection;
using Astral.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Delivery
{
    public class DeliveryManager<TStore> : IDisposable
        where TStore : IStore<TStore>
    {
        private readonly CancellationDisposable _dispose;
        private readonly TimeSpan _leaseInterval;

        private readonly ConcurrentDictionary<Guid, Lease> _leases =
            new ConcurrentDictionary<Guid, Lease>();

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

        public async Task<ILease<TStore>> AddDelivery(Guid deliveryId)
        {
            if (_dispose.IsDisposed) throw new ObjectDisposedException("DeliveryManager");
            if (!await PickupLease(deliveryId))
            {
                var cancellation = new CancellationTokenSource();
                var token = cancellation.Token;
                cancellation.Cancel();
                cancellation.Dispose();
                return new Lease((a, st) => Task.FromResult(LeaseState.NoLease), LeaseState.NoLease, token);
            }
            {
                return _leases.GetOrAdd(deliveryId, _ =>
                {
                    var direct = new CancellationDisposable();
                    var combined = CancellationTokenSource.CreateLinkedTokenSource(direct.Token, _dispose.Token);
                    var dsp = new CompositeDisposable(direct, combined);
                    return new Lease(async (todo, state) =>
                    {
                        _leases.TryRemove(deliveryId, out var _);
                        switch (todo)
                        {
                            case ReleaseAction.CancelType ct:
                                if (state == LeaseState.LeaseTaken)
                                    await DoInScope(p => p
                                        .Where(t => t.DeliveryId == deliveryId && t.Sponsor == _sponsor)
                                        .Set(t => t.Sponsor, (string) null)
                                        .Set(t => t.LeasedTo, DateTimeOffset.Now)
                                        .Update());
                                return LeaseState.LeaseDropped;
                            case ReleaseAction.DeleteType d:
                                await DoInScope(p => p
                                    .Where(t => t.DeliveryId == deliveryId)
                                    .Delete());
                                return LeaseState.LeaseDropped;
                            case ReleaseAction.ArchiveType a:
                                await DoInScope(p => p
                                    .Where(t => t.DeliveryId == deliveryId)
                                    .Set(t => t.Delivered, true)
                                    .Set(t => t.Ttl, a.DeleteAt)
                                    .Set(t => t.Sponsor, (string) null)
                                    .Update());
                                return LeaseState.LeaseDropped;
                            case ReleaseAction.ErrorType e:
                                await DoInScope(p => p
                                    .Where(t => t.DeliveryId == deliveryId && t.Sponsor == _sponsor)
                                    .Set(t => t.LastError, e.Exception.Message)
                                    .Set(t => t.Sponsor, (string) null)
                                    .Set(t => t.LeasedTo, DateTimeOffset.Now)
                                    .Update());
                                return LeaseState.LeaseDropped;
                            case ReleaseAction.RedeliveryType r:
                                await DoInScope(p => p
                                    .Where(t => t.DeliveryId == deliveryId)
                                    .Set(t => t.Delivered, false)
                                    .Set(t => t.Sponsor, (string) null)
                                    .Set(t => t.LeasedTo, r.RedeliveryAt)
                                    .Update());
                                return LeaseState.LeaseDropped;

                           default:
                                throw new ArgumentOutOfRangeException($"Unknown ReleaseAction {todo?.GetType()}");
                        }
                    }, LeaseState.LeaseTaken, combined.Token);
                    
                });
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

        




        private class Lease : ILease<TStore>
        {
            private readonly AsyncLock _locker = new AsyncLock();
            private readonly Func<ReleaseAction, LeaseState, Task<LeaseState>> _releaser;
            private LeaseState _state;

            public Lease(Func<ReleaseAction, LeaseState, Task<LeaseState>> releaser, LeaseState state, CancellationToken token)
            {
                _releaser = releaser;
                _state = state;
                Token = token;
                Token.Register(() => State = LeaseState.LeaseDropped);
            }


            public void Dispose()
            {
                Release(ReleaseAction.Cancel).Wait(CancellationToken.None);
            }

            public LeaseState State 
            {
                get
                {
                    using (_locker.Take().Result)
                        return _state;
                }
                private set
                {
                    using (_locker.Take().Result)
                        _state = value;
                }
            }

            public CancellationToken Token { get; }

            public async Task Release(ReleaseAction action)
            {
                // ReSharper disable once MethodSupportsCancellation
                using (await _locker.Take())
                {
                    _state = await _releaser(action, _state);
                }
            }


        }
    }
}