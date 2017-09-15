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
            builder.RegisterLaw(Law.Axiom(new ServiceTypeSetting(serviceType)));
            var typeInfo = serviceType.GetTypeInfo();
            foreach (var astralAttribute in serviceType.GetCustomAttributes(true).OfType<IConfigAttribute>())
            {
                foreach(var fact in astralAttribute.GetConfigElements(typeInfo))
                    builder.RegisterLaw(Law.Axiom(fact.GetType(), fact));
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

        public static LawBookBuilder AddEndpointLaws(this LawBookBuilder builder, PropertyInfo property)
        {
            var propType = property.PropertyType;
            if (propType.IsConstructedGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(IEvent<>))
                {
                    
                        builder.RegisterLaw(Law.Axiom(new EndpointTypeSetting(EndpointType.Event)));
                        builder.RegisterLaw(Law.Axiom(new EndpointMemberSetting(property)));
                        builder.RegisterLaw(Law.Axiom(new MessageTypeSetting(propType.GenericTypeArguments[0])));
                    
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                            {
                                foreach(var fact in astralAttribute.GetConfigElements(property))
                                    builder.RegisterLaw(Law.Axiom(fact.GetType(), fact));
                            }
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<>))
                {
                    builder.RegisterLaw(Law.Axiom(new EndpointTypeSetting(EndpointType.Call)));
                    builder.RegisterLaw(Law.Axiom(new EndpointMemberSetting(property)));
                    builder.RegisterLaw(Law.Axiom(new MessageTypeSetting(propType.GenericTypeArguments[0])));
                    builder.RegisterLaw(Law.Axiom(new ResponseTypeSetting(typeof(ValueTuple))));
                    foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                    {
                        foreach(var fact in astralAttribute.GetConfigElements(property))
                            builder.RegisterLaw(Law.Axiom(fact.GetType(), fact));
                    }
                    
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<,>))
                {
                    builder.RegisterLaw(Law.Axiom(new EndpointTypeSetting(EndpointType.Call)));
                            builder.RegisterLaw(Law.Axiom(new EndpointMemberSetting(property)));
                            builder.RegisterLaw(Law.Axiom(new MessageTypeSetting(propType.GenericTypeArguments[0])));
                            builder.RegisterLaw(Law.Axiom(new ResponseTypeSetting(propType.GenericTypeArguments[1])));
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                            {
                                foreach (var fact in astralAttribute.GetConfigElements(property))
                                    builder.RegisterLaw(Law.Axiom(fact.GetType(), fact));
                            }
                        
                    return builder;
                }
            }

            throw new ArgumentException($"{property} is not valid endpoint");
        }
    }
}