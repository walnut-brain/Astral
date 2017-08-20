using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAstral(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<DedicatedScopeProvider>();
            serviceCollection.AddTransient(p =>
                p.GetRequiredService<DedicatedScopeProvider>().GetDedicatedScope());
            return serviceCollection;
        }
    }
}