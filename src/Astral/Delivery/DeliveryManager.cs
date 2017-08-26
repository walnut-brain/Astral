using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Delivery
{
    public class DeliveryManager<TStore> : IDisposable
        where TStore : IStore<TStore>
    {
        private readonly CancellationDisposable _dispose;
        private readonly TimeSpan _leaseInterval;

        private readonly ConcurrentDictionary<Guid, DeliveryLease> _leases =
            new ConcurrentDictionary<Guid, DeliveryLease>();

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

        public async Task<IDeliveryLease<TStore>> AddDelivery(Guid deliveryId)
        {
            if (_dispose.IsDisposed) throw new ObjectDisposedException("DeliveryManager");
            if (!await PickupLease(deliveryId))
            {
                var cancellation = new CancellationTokenSource();
                var token = cancellation.Token;
                cancellation.Cancel();
                cancellation.Dispose();
                return new DeliveryLease(token, Disposable.Empty, a => { }, deliveryId, _sponsor);
            }
            {
                return _leases.GetOrAdd(deliveryId, _ =>
                {
                    var direct = new CancellationDisposable();
                    var combined = CancellationTokenSource.CreateLinkedTokenSource(direct.Token, _dispose.Token);
                    var dsp = new CancellationDisposable(combined);
                    return new DeliveryLease(dsp.Token, dsp, act =>
                    {
                        DoInScope(async srv =>
                        {
                            act(srv);
                            await srv.RemoveLease(deliveryId, _sponsor);
                        }).Wait(CancellationToken.None);
                        if (_leases.TryRemove(deliveryId, out var r))
                            r.Kill();
                    }, deliveryId, _sponsor);
                    ;
                });
            }
        }


        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var current = _leases.Keys.ToList();
                var renewed =
                    await DoInScope(async p => await p.RenewLeases(_sponsor, _leaseInterval + _leaseInterval));
                var toRemove = current.Where(p => renewed.All(t => t != p));
                foreach (var guid in toRemove)
                    if (_leases.TryRemove(guid, out var p))
                        p.Kill();
                await Task.Delay(_leaseInterval, token);
            }
        }


        private Task<bool> PickupLease(Guid deliveryId)
        {
            return DoInScope(async srv =>
                await srv.UpdateLease(_sponsor, deliveryId, DateTimeOffset.Now + _leaseInterval + _leaseInterval));
        }

        private Task CleanLeases()
        {
            return DoInScope(async srv => await srv.RemoveLeases(_sponsor));
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

        private class DeliveryCloseOperations : IDeliveryCloseOperations
        {
            private readonly IDeliveryDataService<TStore> _service;
            private readonly Guid _deliveryId;
            private readonly string _sponsor;

            public DeliveryCloseOperations(IDeliveryDataService<TStore> service, Guid deliveryId, string sponsor)
            {
                _service = service;
                _deliveryId = deliveryId;
                _sponsor = sponsor;
            }

            public void Delete() => _service.Delete(_deliveryId);
            public void SetException(Exception exception) => _service.SetException(_deliveryId, exception);

            public void Archive(TimeSpan archiveTime) =>
                _service.Archive(_deliveryId, DateTimeOffset.Now + archiveTime);
        }

        private class DeliveryLease : IDeliveryLease<TStore>
        {
            private readonly Action<Action<IDeliveryDataService<TStore>>> _releaseAction;
            private readonly IDisposable _toDispose;
            private readonly Guid _deliveryId;
            private readonly string _sponsor;

            public DeliveryLease(CancellationToken token,
                IDisposable toDispose,
                Action<Action<IDeliveryDataService<TStore>>> releaseAction, Guid deliveryId, string sponsor)
            {
                Token = token;
                _toDispose = toDispose;
                _releaseAction = releaseAction;
                _deliveryId = deliveryId;
                _sponsor = sponsor;
            }

            public CancellationToken Token { get; }

            public void Release(Action<IDeliveryCloseOperations> action)
            {
                _toDispose.Dispose();
                _releaseAction(p => action(new DeliveryCloseOperations(p, _deliveryId, _sponsor)));
            }

            public void Kill()
            {
                _toDispose.Dispose();
            }
        }
    }
}