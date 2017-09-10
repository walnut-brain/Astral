using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    public interface ITaggedScopeExtender
    {
        void Extend(object tag, IServiceCollection collection);
    }
}