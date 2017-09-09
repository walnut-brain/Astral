using System;
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
        private readonly BusSpecification _specification;
        private readonly CompositeDisposable _disposable;

        public Bus(BusSpecification specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
            _disposable = new CompositeDisposable(specification);
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
            return new BusService<TService>(_specification.Service<TService>());
        }
    }
}