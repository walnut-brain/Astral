using System;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.Lawium;

namespace Astral.Configuration.Configs
{
    public abstract class ServiceConfig : ConfigBase
    {
        protected ServiceConfig(LawBook lawBook) : base(lawBook)
        {
        }

        public Type ServiceType => this.Get<ServiceType>().Value;
        public string ServiceName => this.Get<ServiceName>().Value;



        public EndpointConfig Endpoint(string name)
        {
            var propertyInfo = ServiceType.GetProperty(name);
            if(propertyInfo == null)
                throw new ArgumentException($"{name} is not valid endpoint property name");
            var propType = propertyInfo.PropertyType;
            if (propType.IsConstructedGenericType && propType.GetGenericTypeDefinition() == typeof(IEvent<>))
            {
                var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                    b =>
                    {
                        b.RegisterLaw(Law.Axiom(EndpointType.Event));
                        b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                        b.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    }).Result;
                return new EndpointConfig(book);
            }

            throw new ArgumentException($"{name} is not valid endpoint");

        }
    }

    public class ServiceConfig<T> : ServiceConfig
    {
        internal ServiceConfig(LawBook lawBook) : base(lawBook)
        {
        }

        public EndpointConfig Endpoint<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Event));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TEvent))));
                }).Result;
            return new EndpointConfig(book);
        }
    }
}