using System;
using System.Reactive.Disposables;
using System.Reflection;
using Astral.Configuration.Configs;
using Astral.Internals;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class Bus : IBus
    {
        private readonly BusConfig _config;
        private readonly CompositeDisposable _disposable;

        public Bus(BusConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _disposable = new CompositeDisposable(config);
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
            return new BusService<TService>(_config.Service<TService>());
        }
    }
}