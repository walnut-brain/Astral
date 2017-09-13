using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Astral.Internals;
using Astral.Specifications;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class Bus : IBus
    {
        private readonly BusConfig _config;
        private readonly CompositeDisposable _disposable;
        private readonly ConcurrentDictionary<Type, IBusService> _services = new ConcurrentDictionary<Type, IBusService>();

        public Bus(BusConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _disposable = new CompositeDisposable(config)
            {
                Disposable.Create(() =>
                {
                    while (!_services.IsEmpty)
                    {
                        var type = _services.Keys.FirstOrDefault();
                        if (type == null)
                            continue;
                        if (_services.TryRemove(type, out var service))
                            service.Dispose();
                    }
                })
            };
        }


        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IBusService<TService> Service<TService>()
            where TService : class
        {
            if (_disposable.IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            return (IBusService<TService>) _services.GetOrAdd(typeof(TService), _ => new BusService<TService>(_config.Service<TService>()));
        }
    }
}