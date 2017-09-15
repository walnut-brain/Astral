using System;
using Astral.Configuration;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Specifications
{
    public class BusConfig : ConfigBase, IDisposable
    {
        private readonly TypeEncoding _typeEncoding;
        private readonly Serialization<byte[]> _serialization;
        private readonly TransportProvider _transportProvider;

        internal BusConfig(LawBook lawBook, 
            TypeEncoding typeEncoding, 
            Serialization<byte[]> serialization, 
            TransportProvider transportProvider, 
            IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
        {
            _typeEncoding = typeEncoding;
            _serialization = serialization;
            _transportProvider = transportProvider;
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(TypeEncoding)) return _typeEncoding;
            if (serviceType == typeof(Serialization<byte[]>)) return _serialization;
            if (serviceType == typeof(TransportProvider)) return _transportProvider;
            return base.GetService(serviceType);
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
                .GetOrAddSubBook(serviceType, b => b.AddServiceLaws(serviceType));
            var cfgType = typeof(ServiceConfig<>).MakeGenericType(serviceType);
            return (ServiceConfig) Activator.CreateInstance(cfgType, this);
        }
        
        public void Dispose()
        {
            this.GetService<TransportProvider>().Dispose();
        }
    }
}