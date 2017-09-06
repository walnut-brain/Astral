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
            Func<BusConfig, TBus> factory, string systemName,  Action<BusBuilder> configure)
            where TInterface : class, IBus
            where TBus : Bus, TInterface
        {
            return 
                serviceCollection.AddSingleton<TInterface, TBus>(sp =>
                {
                    var builder = new BusBuilder(sp, systemName);
                    configure(builder);
                    var config = builder.Build();
                    return factory(config);
                });
            
        }

        public static IServiceCollection AddBus(this IServiceCollection serviceCollection, string systemName,
            Action<BusBuilder> configure)
            => AddBus<Bus, IBus>(serviceCollection, config => new Bus(config), systemName, configure);
    }
}