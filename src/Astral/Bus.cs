using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;
using Astral.Configuration.Configs;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class Bus : IBus
    {
        private readonly ILoggerFactory _factory;
        private readonly BusConfig _config;
        private readonly CompositeDisposable _disposable;

        public Bus(ILoggerFactory factory, BusConfig config)
        {
            _factory = factory;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _disposable = new CompositeDisposable(config);
        }

        


        public void Dispose()
        {
            _disposable.Dispose();
        }

        public BusService<TService> Service<TService>()
            where TService : class
        {
            if (_disposable.IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            return new BusService<TService>(_config.Service<TService>(), _factory);
        }
    }

    public interface IBus : IDisposable
    {
        BusService<T> Service<T>() where T : class;
    }

    public interface IBusService<T> : IBusService
        where T : class
    {
        
    }

    public interface IBusService
    {
//        IBusEndpoint Endpoint(PropertyInfo property);

    }

    
}