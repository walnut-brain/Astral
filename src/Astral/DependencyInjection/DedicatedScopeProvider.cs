using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    internal class DedicatedScopeProvider 
    {
        private readonly IServiceContainer _container;

        public DedicatedScopeProvider(IServiceContainer container)
        {
            _container = container;
        }
        
        public IDedicatedScope GetDedicatedScope()
            => new DedicatedScope(_container.CreateScope());
        
    }
}