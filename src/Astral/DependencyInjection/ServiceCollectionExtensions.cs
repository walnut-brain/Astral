using System;
using Astral.Configuration.Builders;
using Astral.Configuration.Configs;
using Astral.Delivery;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAstral(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<DedicatedScopeProvider>();
            serviceCollection.AddTransient<Func<IDedicatedScope>>(p =>
                () => p.GetRequiredService<DedicatedScopeProvider>().GetDedicatedScope());
            serviceCollection.AddSingleton(typeof(DeliveryManager<>));
            return serviceCollection;
        }

        public static IServiceCollection AddBus<TBus, TInterface>(this IServiceCollection serviceCollection,
            BusBuilder builder, Func<IServiceProvider, BusConfig, TBus> factory)
            where TInterface : class
            where TBus : Bus, TInterface
            => serviceCollection.AddSingleton<TInterface, TBus>(sp =>
                builder.Build(sp, factory));

        public static IServiceCollection AddBus(this IServiceCollection serviceCollection, BusBuilder builder)
            => serviceCollection.AddSingleton(builder.Build);
    }
}