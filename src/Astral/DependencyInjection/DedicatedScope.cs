using System;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    internal class DedicatedScope : IDedicatedScope
    {
        private readonly IServiceScope _scope;

        public DedicatedScope(IServiceScope scope)
        {
            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        ~DedicatedScope()
        {
            Dispose();
        }
    }
}