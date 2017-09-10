using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    public interface ITaggedScopeProvider
    {
        IServiceScope CreateScope(object tag);
    }
    
    
}