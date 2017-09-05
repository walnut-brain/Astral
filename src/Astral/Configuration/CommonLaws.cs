using System;
using System.Linq;
using System.Reflection;
using Astral.Configuration.Settings;
using Lawium;

namespace Astral.Configuration
{
    internal static class CommonLaws
    {
        public static LawBookBuilder AddServiceLaws(this LawBookBuilder builder, Type serviceType)
        {
            builder.RegisterLaw(Law.Axiom(new ServiceType(serviceType)));
            var typeInfo = serviceType.GetTypeInfo();
            foreach (var astralAttribute in serviceType.GetCustomAttributes(true).OfType<IAstralAttribute>())
            {
                var (atype, value) = astralAttribute.GetConfigElement(typeInfo);
                builder.RegisterLaw(Law.Axiom(atype, value));
            }
            return builder;
        }

        public static LawBookBuilder AddEndpointLaws(this LawBookBuilder builder, PropertyInfo property)
        {
            var propType = property.PropertyType;
            if (propType.IsConstructedGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(IEvent<>))
                {
                    
                        builder.RegisterLaw(Law.Axiom(EndpointType.Event));
                        builder.RegisterLaw(Law.Axiom(new EndpointMember(property)));
                        builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IAstralAttribute>())
                            {
                                var (atype, value) = astralAttribute.GetConfigElement(property);
                                builder.RegisterLaw(Law.Axiom(atype, value));
                            }
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<>))
                {
                    builder.RegisterLaw(Law.Axiom(EndpointType.Call));
                    builder.RegisterLaw(Law.Axiom(new EndpointMember(property)));
                    builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    builder.RegisterLaw(Law.Axiom(new ResponseType(typeof(ValueTuple))));
                    foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IAstralAttribute>())
                    {
                        var (atype, value) = astralAttribute.GetConfigElement(property);
                        builder.RegisterLaw(Law.Axiom(atype, value));
                    }
                    
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<,>))
                {
                    builder.RegisterLaw(Law.Axiom(EndpointType.Call));
                            builder.RegisterLaw(Law.Axiom(new EndpointMember(property)));
                            builder.RegisterLaw(Law.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                            builder.RegisterLaw(Law.Axiom(new ResponseType(propType.GenericTypeArguments[1])));
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IAstralAttribute>())
                            {
                                var (atype, value) = astralAttribute.GetConfigElement(property);
                                builder.RegisterLaw(Law.Axiom(atype, value));
                            }
                        
                    return builder;
                }
            }

            throw new ArgumentException($"{property} is not valid endpoint");
        }
    }
}