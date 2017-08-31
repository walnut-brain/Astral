using System;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Astral.Markup;
using Lawium;

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
            if (propertyInfo == null)
                throw new ArgumentException($"{name} is not valid endpoint property name");
            var propType = propertyInfo.PropertyType;
            if (propType.IsConstructedGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(IEvent<>))
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
                if (propType.GetGenericTypeDefinition() == typeof(ICall<>))
                {
                    var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                        b =>
                        {
                            b.RegisterLaw(Law.Axiom(EndpointType.Call));
                            b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                            b.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                            b.RegisterLaw(Law.Axiom(new ResponseType(typeof(ValueTuple))));
                        }).Result;
                    return new EndpointConfig(book);
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<,>))
                {
                    var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                        b =>
                        {
                            b.RegisterLaw(Law.Axiom(EndpointType.Call));
                            b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                            b.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                            b.RegisterLaw(Law.Axiom(new ResponseType(propType.GenericTypeArguments[1])));
                        }).Result;
                    return new EndpointConfig(book);
                }
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
        
        public EndpointConfig Endpoint<TArgs>(Expression<Func<T, ICall<TArgs>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Call));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TArgs))));
                    b.RegisterLaw(Law.Axiom(new ResponseType(typeof(ValueTuple))));
                }).Result;
            return new EndpointConfig(book);
        }
        
        public EndpointConfig Endpoint<TArgs, TResult>(Expression<Func<T, ICall<TArgs, TResult>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var book = LawBook.GetOrAddSubBook(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Call));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TArgs))));
                    b.RegisterLaw(Law.Axiom(new ResponseType(typeof(TResult))));
                }).Result;
            return new EndpointConfig(book);
        }
    }
}