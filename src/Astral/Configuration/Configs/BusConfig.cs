using System;
using System.Linq;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Lawium;

namespace Astral.Configuration.Configs
{
    public class BusConfig : ConfigBase, IDisposable
    {

        internal BusConfig(LawBook lawBook, TypeEncoding typeEncoding, Serializer<byte[]> serializer, TransportProvider transportProvider, IServiceProvider provider) : base(lawBook, provider)
        {
            Transports = transportProvider;
            TypeEncoding = typeEncoding;
            Serializer = serializer;
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
            return (ServiceConfig) Activator.CreateInstance(cfgType, book, TypeEncoding, Serializer, Transports);
        }
        
        public TypeEncoding TypeEncoding { get; }
        public Serializer<byte[]> Serializer { get; }
        
        internal TransportProvider Transports { get; }

        public void Dispose()
        {
            Transports.Dispose();
        }
    }
}