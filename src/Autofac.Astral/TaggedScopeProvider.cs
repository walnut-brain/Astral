using System;
using System.Collections.Generic;
using Astral;
using Astral.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Astral
{
    public class TaggedScopeProvider : ITaggedScopeProvider
    {
        private readonly ILifetimeScope _scope;

        public TaggedScopeProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public IServiceScope CreateScope(object tag)
        {
            var serviceCollection = new ServiceCollection();
            foreach (var scopeExtender in _scope.Resolve<IEnumerable<ITaggedScopeExtender>>())
            {
                scopeExtender.Extend(tag, serviceCollection);
            }
            
            return new AutofacServiceScope(_scope.BeginLifetimeScope(tag, cb => cb.Populate(serviceCollection)));
        }
        
        /// <summary>
        /// Autofac implementation of the ASP.NET Core <see cref="IServiceScope"/>.
        /// </summary>
        /// <seealso cref="Extensions.DependencyInjection.IServiceScope" />
        private class AutofacServiceScope : IServiceScope
        {
            private readonly ILifetimeScope _lifetimeScope;

            /// <summary>
            /// Initializes a new instance of the <see cref="AutofacServiceScope"/> class.
            /// </summary>
            /// <param name="lifetimeScope">
            /// The lifetime scope from which services should be resolved for this service scope.
            /// </param>
            public AutofacServiceScope(ILifetimeScope lifetimeScope)
            {
                this._lifetimeScope = lifetimeScope;
                this.ServiceProvider = this._lifetimeScope.Resolve<IServiceProvider>();
            }

            /// <summary>
            /// Gets an <see cref="IServiceProvider" /> corresponding to this service scope.
            /// </summary>
            /// <value>
            /// An <see cref="IServiceProvider" /> that can be used to resolve dependencies from the scope.
            /// </value>
            public IServiceProvider ServiceProvider { get; }

            /// <summary>
            /// Disposes of the lifetime scope and resolved disposable services.
            /// </summary>
            public void Dispose()
            {
                this._lifetimeScope.Dispose();
            }
        }
    }
}