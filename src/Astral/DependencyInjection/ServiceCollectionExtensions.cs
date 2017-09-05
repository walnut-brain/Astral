using System;
using Astral.Configuration.Builders;
using Astral.Configuration.Configs;
using Astral.Deliveries;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAstral(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(BoundDeliveryManager<>));
            return serviceCollection;
        }

        public static IServiceCollection AddBus<TBus, TInterface>(this IServiceCollection serviceCollection,
            BusBuilder builder, Func<ILoggerFactory, BusConfig, TBus> factory)
            where TInterface : class
            where TBus : Bus, TInterface
        {
            return serviceCollection.AddSingleton<TInterface, TBus>(sp =>
                builder.Build(sp, factory));
        }

        public static IServiceCollection AddBus(this IServiceCollection serviceCollection,
            BusBuilder builder) 
        {
            return serviceCollection.AddSingleton(builder.Build);
        }
    }
}