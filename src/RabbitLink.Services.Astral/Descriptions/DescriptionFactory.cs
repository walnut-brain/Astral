using System;
using System.Net.Mime;
using System.Reflection;
using Astral;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using RabbitLink.Services.Descriptions;
using RabbitLink.Topology;

namespace RabbitLink.Services.Astral.Descriptions
{
    public class DescriptionFactory : IDescriptionFactory
    {
        protected ExchangeAttribute GetDefaultExchange(Type type)
        {
            var owner = type.GetCustomAttribute<OwnerAttribute>()?.OwnerName ??
                        throw new AstralException("Owner not specified");
            var serviceName = type.GetCustomAttribute<ServiceAttribute>()?.Name ??
                              throw new AstralException("Service name not specified");
            return new ExchangeAttribute($"{owner}.{serviceName}");
        }

        private LinkExchangeType GetLinkExchangeType(ExchangeKind kind)
        {
            switch (kind)
            {
                case ExchangeKind.Fanout:
                    return LinkExchangeType.Fanout;
                case ExchangeKind.Direct:
                    return LinkExchangeType.Direct;
                case ExchangeKind.Topic:
                    return LinkExchangeType.Topic;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
        
        public ServiceDescription GetDescription(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if(!type.IsInterface) throw new ArgumentException($"Type {type} must be a interface");
            var serviceName = type.GetCustomAttribute<ServiceAttribute>()?.Name ??
                              throw new AstralException("No service attribute found");
            var serviceOwner = type.GetCustomAttribute<OwnerAttribute>()?.OwnerName ??
                              throw new AstralException("No owner attribute found");
            var description = new ServiceDescription(serviceName, serviceOwner);
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(EventHandler<>))
                {
                    var exchange = property.GetCustomAttribute<ExchangeAttribute>() ??
                                   type.GetCustomAttribute<ExchangeAttribute>() ?? GetDefaultExchange(type);
                    string routingKey = null;
                    if (exchange.Kind != ExchangeKind.Fanout)
                        routingKey = property.GetCustomAttribute<RoutingKeyAttribute>()?.Key ??
                                     property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                     throw new AstralException("Missign routing key or endpoint name");
                    var contentType = property.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      type.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      new ContentType("text/json;charset=utf-8");

                    var endpointName = property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                       throw new AstralException($"Not endpoint attribute found on {property.Name}");
                    
                    var eventDesc = new EventDescription(description, endpointName,  new ExchangeDescription(exchange.Name)
                        {
                            Type = GetLinkExchangeType(exchange.Kind) 
                        }, routingKey, property.PropertyType.GenericTypeArguments[0], contentType);
                    description.Events.Add(property.Name, eventDesc);
                }
            }
            return description;
        }
    }
}