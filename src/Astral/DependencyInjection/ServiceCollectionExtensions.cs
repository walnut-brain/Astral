using System;
using Astral.Configuration.Builders;
using Astral.Configuration.Configs;
using Astral.Delivery;
using Astral.Transport;
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

        public static IServiceCollection AddBus<TBus, TTransport, TInterface>(this IServiceCollection serviceCollection,
            BusBuilder builder, Func<IServiceProvider, BusConfig, TBus> factory)
            where TInterface : class
            where TBus : Bus<TTransport>, TInterface
            where TTransport : class, ITransport
        {
            return serviceCollection.AddSingleton<TInterface, TBus>(sp =>
                builder.Build<TBus, TTransport>(sp, factory));
        }

        public static IServiceCollection AddBus<TTransport>(this IServiceCollection serviceCollection,
            BusBuilder builder) where TTransport : class, ITransport
        {
            return serviceCollection.AddSingleton(builder.Build<TTransport>);
        }
    }
}