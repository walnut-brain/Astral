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
        public static LawBookBuilder<Fact> AddServiceLaws(this LawBookBuilder<Fact> builder, Type serviceType)
        {
            builder.RegisterLaw(Law<Fact>.Axiom(new ServiceType(serviceType)));
            var typeInfo = serviceType.GetTypeInfo();
            foreach (var astralAttribute in serviceType.GetCustomAttributes(true).OfType<IConfigAttribute>())
            {
                foreach(var fact in astralAttribute.GetConfigElements(typeInfo))
                    builder.RegisterLaw(Law<Fact>.Axiom(fact.GetType(), fact));
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

        public static LawBookBuilder<Fact> AddEndpointLaws(this LawBookBuilder<Fact> builder, PropertyInfo property)
        {
            var propType = property.PropertyType;
            if (propType.IsConstructedGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(IEvent<>))
                {
                    
                        builder.RegisterLaw(Law<Fact>.Axiom(new EndpointKind(EndpointType.Event)));
                        builder.RegisterLaw(Law<Fact>.Axiom(new EndpointMember(property)));
                        builder.RegisterLaw(Law<Fact>.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                            {
                                foreach(var fact in astralAttribute.GetConfigElements(property))
                                    builder.RegisterLaw(Law<Fact>.Axiom(fact.GetType(), fact));
                            }
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<>))
                {
                    builder.RegisterLaw(Law<Fact>.Axiom(new EndpointKind(EndpointType.Call)));
                    builder.RegisterLaw(Law<Fact>.Axiom(new EndpointMember(property)));
                    builder.RegisterLaw(Law<Fact>.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                    builder.RegisterLaw(Law<Fact>.Axiom(new ResponseType(typeof(ValueTuple))));
                    foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                    {
                        foreach(var fact in astralAttribute.GetConfigElements(property))
                            builder.RegisterLaw(Law<Fact>.Axiom(fact.GetType(), fact));
                    }
                    
                    return builder;
                }
                if (propType.GetGenericTypeDefinition() == typeof(ICall<,>))
                {
                    builder.RegisterLaw(Law<Fact>.Axiom(new EndpointKind(EndpointType.Call)));
                            builder.RegisterLaw(Law<Fact>.Axiom(new EndpointMember(property)));
                            builder.RegisterLaw(Law<Fact>.Axiom(new MessageType(propType.GenericTypeArguments[0])));
                            builder.RegisterLaw(Law<Fact>.Axiom(new ResponseType(propType.GenericTypeArguments[1])));
                            foreach (var astralAttribute in property.GetCustomAttributes(true).OfType<IConfigAttribute>())
                            {
                                foreach (var fact in astralAttribute.GetConfigElements(property))
                                    builder.RegisterLaw(Law<Fact>.Axiom(fact.GetType(), fact));
                            }
                        
                    return builder;
                }
            }

            throw new ArgumentException($"{property} is not valid endpoint");
        }
    }
}