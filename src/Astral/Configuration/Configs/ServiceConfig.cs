using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Lawium;

namespace Astral.Configuration.Configs
{
    public abstract class ServiceConfig : ConfigBase
    {
        public TypeEncoding TypeEncoding { get; }
        public Serializer<byte[]> Serializer { get; }

        internal ServiceConfig(LawBook lawBook, TypeEncoding typeEncoding, Serializer<byte[]> serializer, TransportProvider transports, IServiceProvider provider) : base(lawBook, provider)
        {
            TypeEncoding = typeEncoding;
            Serializer = serializer;
            Transports = transports;
        }

        public Type ServiceType => this.Get<ServiceType>().Value;
        public string ServiceName => this.Get<ServiceName>().Value;

        public EndpointConfig Endpoint(string name)
        {
            var propertyInfo = ServiceType.GetProperty(name);
            if (propertyInfo == null)
                throw new ArgumentException($"{name} is not valid endpoint property name");
            return Endpoint(propertyInfo);
        }

        protected EndpointConfig Endpoint(PropertyInfo propertyInfo)
        {
            var book = LawBook.GetOrAddSubBook(propertyInfo.Name, b => b.AddEndpointLaws(propertyInfo)).Result;
            return new EndpointConfig(book, TypeEncoding, Serializer, Transports, Provider);
        }
        
        internal TransportProvider Transports { get; }
    }

    public class ServiceConfig<T> : ServiceConfig
    {
        internal ServiceConfig(LawBook lawBook, TypeEncoding typeEncoding, Serializer<byte[]> serializer, TransportProvider transportProvider, IServiceProvider provider) : base(lawBook, typeEncoding, serializer, transportProvider, provider)
        {
        }

        public EndpointConfig Endpoint<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector)
            => Endpoint(selector.GetProperty());
            
        
        public EndpointConfig Endpoint<TArgs>(Expression<Func<T, ICall<TArgs>>> selector)
            => Endpoint(selector.GetProperty());
        
        public EndpointConfig Endpoint<TArgs, TResult>(Expression<Func<T, ICall<TArgs, TResult>>> selector)
            => Endpoint(selector.GetProperty());
    }
}