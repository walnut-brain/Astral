using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<Guid, CancellationDisposable> _leases = new ConcurrentDictionary<Guid, CancellationDisposable>(); 
        

        public DeliveryManager(Func<IDedicatedScope> scopeProvider, TimeSpan leaseInterval)
        {
            _scopeProvider = scopeProvider;
            _leaseInterval = leaseInterval;
            _sponsor = $"{Environment.MachineName}-{Guid.NewGuid()}";
            _dispose = new CancellationDisposable();
            _renewLoop = Loop(_dispose.Token);
        }

        public async Task<IDeliveryLease<TUoW>> AddDelivery(Guid deliveryId)
        {
            if(_dispose.IsDisposed) throw new ObjectDisposedException("DeliveryManager");
            if (!await PickupLease(deliveryId))
            {
                var cancellation = new CancellationTokenSource();
                var token = cancellation.Token;
                cancellation.Cancel();
                cancellation.Dispose();
                return new DeliveryLease(token, a => { });
            }
            {
                var cancellation = _leases.GetOrAdd(deliveryId, _ =>
                {
                    var direct = new CancellationDisposable();
                    var combined = CancellationTokenSource.CreateLinkedTokenSource(direct.Token, _dispose.Token);
                    var dsp = new CancellationDisposable(combined);
                    return dsp;
                });
                return new DeliveryLease(cancellation.Token, act =>
                {
                    DoInScope(async srv =>
                    {
                        act(srv);
                        await srv.RemoveLease(deliveryId, _sponsor);
                    });
                    if(_leases.TryRemove(deliveryId, out var r)) 
                        r.Dispose();
                });
            }
        }
        
        
        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var current = _leases.Keys.ToList();
                var renewed = await DoInScope(async p => await p.RenewLeases(_sponsor, _leaseInterval + _leaseInterval));
                var toRemove = current.Where(p => renewed.All(t => t != p));
                foreach (var guid in toRemove)
                {
                    if(_leases.TryRemove(guid, out var p))
                        p.Dispose();
                }
                await Task.Delay(_leaseInterval, token);
            }
        }
    

        public void Dispose()
        {
            if (_dispose.IsDisposed) return;
            _dispose.Dispose();
            _renewLoop.Wait();
            CleanLeases().Wait();
        }



        private Task<bool> PickupLease(Guid deliveryId)
            => DoInScope(async srv => await srv.UpdateLease(_sponsor, deliveryId, DateTimeOffset.Now + _leaseInterval + _leaseInterval));

        private Task CleanLeases() => DoInScope(async srv => await srv.RemoveLeases(_sponsor));
            
        
        
        
        
        private async Task<T> DoInScope<T>(Func<IDeliveryDataService<TUoW>, Task<T>> func)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider<TUoW>>();
                using (var uow = await provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TUoW>>();
                    var result = await func(dataService);
                    await uow.Commit();
                    return result;
                }
            }
        }
        
        private async Task DoInScope(Func<IDeliveryDataService<TUoW>, Task> action)
        {
            using (var scope = _scopeProvider())
            {
                var provider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider<TUoW>>();
                using (var uow = await provider.BeginWork())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDeliveryDataService<TUoW>>();
                    await action(dataService);
                    await uow.Commit();
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