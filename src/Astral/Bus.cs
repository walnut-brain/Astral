using System;
using System.Reactive.Disposables;
using Astral.Configuration.Configs;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class Bus<TTransport> : IDisposable
        where TTransport : class, ITransport
    {
        private readonly BusConfig _config;
        private readonly CompositeDisposable _disposable;
        private readonly IServiceProvider _serviceProvider;
        private readonly TTransport _transport;

        public Bus(IServiceProvider serviceProvider, BusConfig config)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _transport = null;
            _disposable = new CompositeDisposable();
        }

        public Bus(IServiceProvider serviceProvider, BusConfig config, TTransport transport)
            : this(serviceProvider, config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _transport = transport;
            if (_transport is IDisposable d)
                _disposable.Add(d);
        }

        private TTransport Transport => _transport ?? _serviceProvider.GetRequiredService<TTransport>();


        public void Dispose()
        {
            _disposable.Dispose();
        }

        public BusService<TTransport, TService> Service<TService>()
            where TService : class
        {
            if (_disposable.IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            return new BusService<TTransport, TService>(_config.Service<TService>(), Transport,
                _serviceProvider.GetService<ILogger<BusService<TTransport, TService>>>(), _serviceProvider);
        }
    }
}