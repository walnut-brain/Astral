using System;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Configuration.Configs
{
    public class BusConfig : ConfigBase, IDisposable
    {
        internal BusConfig(LawBook<Fact> lawBook, 
            TypeEncoding typeEncoding, 
            Serializer<byte[]> serializer, 
            TransportProvider transportProvider, 
            IServiceProvider serviceProvider) : base(lawBook, p =>
        {
            if (p == typeof(TypeEncoding)) return typeEncoding;
            if (p == typeof(Serializer<byte[]>)) return serializer;
            if (p == typeof(TransportProvider)) return transportProvider;
            return serviceProvider.GetService(p);
        })
        {
        }

        public ServiceConfig<TService> Service<TService>()
        {
            return (ServiceConfig<TService>) Service(typeof(TService));
        }

        public ServiceConfig Service(Type serviceType)
        {
            if (!serviceType.IsInterface)
                throw new ArgumentException($"{serviceType} must be interface");
            var book = LawBook
                .GetOrAddSubBook(serviceType, b => b.AddServiceLaws(serviceType)).Result;
            var cfgType = typeof(ServiceConfig<>).MakeGenericType(serviceType);
            return (ServiceConfig) Activator.CreateInstance(cfgType, this);
        }
        
        public void Dispose()
        {
            this.GetService<TransportProvider>().Dispose();
        }
    }
}