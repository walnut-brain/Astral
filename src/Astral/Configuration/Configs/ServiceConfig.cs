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
        internal ServiceConfig(LawBook lawBook, IServiceProvider provider) : base(lawBook, provider.GetService)
        {
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
            return new EndpointConfig(book, this);
        }
        
    }

    public class ServiceConfig<T> : ServiceConfig
    {
        internal ServiceConfig(LawBook lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
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