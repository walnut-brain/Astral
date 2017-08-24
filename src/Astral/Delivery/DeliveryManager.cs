using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Settings;
using Astral.Data;
using Astral.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Delivery
{
    public class DeliveryManager<TUoW> : IDisposable
        where TUoW : IUnitOfWork
    {
        private readonly Func<IDedicatedScope> _scopeProvider;
        private readonly TimeSpan _leaseInterval;
        private readonly string _sponsor;
        private readonly Task _renewLoop;
        private readonly CancellationDisposable _dispose;
        private readonly Dictionary<Guid, IDisposable> _leases = new Dictionary<Guid, IDisposable>(); 
        

        public DeliveryManager(Func<IDedicatedScope> scopeProvider, TimeSpan leaseInterval)
        {
            _scopeProvider = scopeProvider;
            _leaseInterval = leaseInterval;
            _sponsor = $"{Environment.MachineName}-{Guid.NewGuid()}";
            _dispose = new CancellationDisposable();

        }

        public IDeliveryLease<TUoW> AddDelivery(Guid deliveryId)
        {
            if(_dispose.IsDisposed) throw new ObjectDisposedException("DeliveryManager");
            if (!PickupLease(deliveryId))
            {
                var cancellation = new CancellationTokenSource();
                var token = cancellation.Token;
                cancellation.Cancel();
                cancellation.Dispose();
                return new DeliveryLease(token, a => { });
            }
            {
                var cancellation = new CancellationDisposable();
                var combined = CancellationTokenSource.CreateLinkedTokenSource(cancellation.Token, _dispose.Token);
                lock (_locker)
                {
                    _leases.Add(deliveryId, Disposable.Create(() =>
                    {
                        cancellation.Dispose();
                        combined.Dispose();
                    }));
                }
                return new DeliveryLease(combined.Token, act =>
                {
                    DoInScope(srv =>
                    {
                        act(srv);
                        srv.RemoveLease(deliveryId, _sponsor);
                    });
                    lock(_locker)
                        if (_leases.TryGetValue(deliveryId, out var d))
                        {
                            d.Dispose();
                            _leases.Remove(deliveryId);
                        }
                });
            }
        }

        public void Dispose()
        {
            _dispose.Dispose();
            _renewLoop.Wait();
            CleanLeases();
        }



        private bool PickupLease(Guid deliveryId)
            => DoInScope(srv => srv.UpdateLease(_sponsor, deliveryId, DateTimeOffset.Now + _leaseInterval + _leaseInterval));

        private void CleanLeases() => DoInScope(srv => srv.RemoveLeases(_sponsor));
            
        

        private T DoInScope<T>(Func<IDeliveryDataService<TUoW>, T> func)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider<TUoW>>();
                using (var uow = provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TUoW>>();
                    var result = func(dataService);
                    uow.Commit();
                    return result;
                }
            }
        }
        
        private void DoInScope(Action<IDeliveryDataService<TUoW>> action)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider<TUoW>>();
                using (var uow = provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TUoW>>();
                    action(dataService);
                    uow.Commit();
                }
            }
        }

        private class DeliveryLease : IDeliveryLease<TUoW>
        {
            private readonly Action<Action<IDeliveryDataService<TUoW>>> _action;
            
            public DeliveryLease(CancellationToken token, Action<Action<IDeliveryDataService<TUoW>>> action)
            {
                Token = token;
                _action = action;
            }

            public CancellationToken Token { get; }
            public void Release(Action<IDeliveryDataService<TUoW>> action) => _action(action);

        }
    }

    public interface IDeliveryLease<TUoW>
        where TUoW : IUnitOfWork
    {
        CancellationToken Token { get; }
        void Release(Action<IDeliveryDataService<TUoW>> action);
    }
}