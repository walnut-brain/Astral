using System;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    public static class ServiceProviderExtension
    {
        public static IServiceScope CreateScope(this IServiceProvider provider, object tag)
        {
            var taggedProvider = provider.GetRequiredService<ITaggedScopeProvider>();
            return taggedProvider.CreateScope(tag);
        }
    }
}