using System;
using System.Linq;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Lawium;

namespace Astral.Configuration
{
    internal static class CommonLaws
    {
        public static LawBookBuilder AddServiceLaws(this LawBookBuilder builder, Type serviceType)
        {
            builder.RegisterLaw(Law.Axiom(new ServiceType(serviceType)));
            var typeInfo = serviceType.GetTypeInfo();
            foreach (var attr in serviceType.GetCustomAttributes(true).OfType<Attribute>().Where(IsConfigRegisterAttribute))
            {
                
                    builder.RegisterLaw(Law.Axiom(attr.GetType(), attr));
            }
            return builder;
        }

        public static Acknowledge DefaultExceptionPolicy(Exception exception)
        {
            switch (exception)
            {
                case AcknowledgeException acke:
                    return acke.Acknowledge;
                default:
                    if (exception.IsCancellation())
                        return Acknowledge.Requeue;
                    return Acknowledge.Nack;
            }
        }

        public static bool IsConfigRegisterAttribute(this Attribute attr)
            => attr != null && attr.GetType().GetCustomAttribute<ConfigRegisterAttribute>() != null;

        public static LawBookBuilder AddEndpointLaws(this LawBookBuilder builder, PropertyInfo property)
        {
            var propType = property.PropertyType;
            if (propType.IsConstructedGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(IEvent<>))
                {
                    
                        builder.RegisterLaw(Law.Axiom(EndpointType.Event));
                        builder.RegisterLaw(Law.Axiom(new EndpointProperty(property)));
                        builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    
                            foreach (var attr in property.GetCustomAttributes(true).OfType<Attribute>().Where(IsConfigRegisterAttribute))
                            {
                                builder.RegisterLaw(Law.Axiom(attr.GetType(), attr));
                            }
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<>))
                {
                    builder.RegisterLaw(Law.Axiom(EndpointType.Call));
                    builder.RegisterLaw(Law.Axiom(new EndpointProperty(property)));
                    builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    builder.RegisterLaw(Law.Axiom(new ResponseType(typeof(ValueTuple))));
                    foreach (var attr in property.GetCustomAttributes(true).OfType<Attribute>().Where(IsConfigRegisterAttribute))
                    {
                        builder.RegisterLaw(Law.Axiom(attr.GetType(), attr));
                    }
                    
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<,>))
                {
                    builder.RegisterLaw(Law.Axiom(EndpointType.Call));
                            builder.RegisterLaw(Law.Axiom(new EndpointProperty(property)));
                            builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                            builder.RegisterLaw(Law.Axiom(new ResponseType(propType.GenericTypeArguments[1])));
                            foreach (var attr in property.GetCustomAttributes(true).OfType<Attribute>().Where(IsConfigRegisterAttribute))
                            {
                                builder.RegisterLaw(Law.Axiom(attr.GetType(), attr));
                            }
                        
                    return builder;
                }
            }

            throw new ArgumentException($"{property} is not valid endpoint");
        }
    }
}