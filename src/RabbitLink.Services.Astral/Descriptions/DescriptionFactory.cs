using System;
using System.Collections.Concurrent;
using System.Net.Mime;
using System.Reflection;
using Astral;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using RabbitLink.Services.Descriptions;
using RabbitLink.Topology;

namespace RabbitLink.Services.Astral.Descriptions
{
    /// <summary>
    /// Default description factory realization for astral markup
    /// </summary>
    public class DescriptionFactory : IDescriptionFactory
    {
        /// <summary>
        /// default exchange for service provider
        /// </summary>
        /// <param name="type">service type</param>
        /// <returns>exchange</returns>
        /// <exception cref="SchemaMarkupException">when schema markup is invalid</exception>
        protected virtual ExchangeAttribute GetDefaultExchange(Type type)
        {
            var owner = type.GetCustomAttribute<OwnerAttribute>().OwnerName;
            var serviceName = type.GetCustomAttribute<ServiceAttribute>().Name;
            var exchange = type.GetCustomAttribute<ExchangeAttribute>();
            if (exchange != null)
            {
                if(exchange.Kind == ExchangeKind.Fanout)
                    throw new SchemaMarkupException("Exchange attribute specify fanout exchange on service level");
                if (string.IsNullOrWhiteSpace(exchange.Name))
                    exchange.Name = $"{owner}.{serviceName}";
                return exchange;
            }
            return new ExchangeAttribute($"{owner}.{serviceName}");
        }
        
        /// <summary>
        /// default response exchange for service provider
        /// </summary>
        /// <param name="type">service type</param>
        /// <returns>response exchange</returns>
        /// <exception cref="SchemaMarkupException">when schema markup is invalid</exception>
        protected virtual ResponseExchangeAttribute GetDefaultResponseExchange(Type type)
        {
            var owner = type.GetCustomAttribute<OwnerAttribute>().OwnerName;
            var serviceName = type.GetCustomAttribute<ServiceAttribute>().Name;
            var responseExchange = type.GetCustomAttribute<ResponseExchangeAttribute>();
            if (responseExchange != null)
            {
                if(responseExchange.Kind == ExchangeKind.Fanout)
                    throw new SchemaMarkupException("Response exchange attribute specify fanout exchange on service level");
                if (string.IsNullOrWhiteSpace(responseExchange.Name))
                    responseExchange.Name = $"{owner}.{serviceName}.responses";
                return responseExchange;
            }
            var exchange = GetDefaultExchange(type);
            return new ResponseExchangeAttribute
            {
                Name = exchange.Name,
                Kind = exchange.Kind,
                Durable = exchange.Durable,
                Alternate = exchange.Alternate,
                AutoDelete = exchange.AutoDelete,
                Delayed = exchange.Delayed
            };
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
        
        
        /// <summary>
        /// Generate service description for type
        /// </summary>
        /// <param name="type">service interface type</param>
        /// <returns>service description</returns>
        protected virtual ServiceDescription GenerateDescription(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if(!type.IsInterface) throw new ArgumentException($"Type {type} must be a interface");
            var serviceName = type.GetCustomAttribute<ServiceAttribute>()?.Name ??
                              throw new SchemaMarkupException("No service attribute found");
            var serviceOwner = type.GetCustomAttribute<OwnerAttribute>()?.OwnerName ??
                              throw new SchemaMarkupException("No owner attribute found");
            var description = new ServiceDescription(serviceName, serviceOwner);
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(EventHandler<>))
                {
                    var endpointName = property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                       throw new SchemaMarkupException($"No endpoint attribute found on {property.Name}");
                    var exchange = property.GetCustomAttribute<ExchangeAttribute>();
                    if (exchange != null)
                    {
                        if (string.IsNullOrWhiteSpace(exchange.Name))
                            exchange.Name = $"{serviceOwner}.{serviceName}.{endpointName}";
                    }
                    else
                        exchange = GetDefaultExchange(type);
                    
                    string routingKey = null;
                    if (exchange.Kind != ExchangeKind.Fanout)
                    {
                        routingKey = property.GetCustomAttribute<RoutingKeyAttribute>()?.Key ??
                                     property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                     throw new SchemaMarkupException("Missign routing key or endpoint name");
                    }
                    var contentType = property.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      type.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      new ContentType("text/json;charset=utf-8");

                    
                    
                    var eventDesc = new EventDescription(description, endpointName, property.PropertyType.GenericTypeArguments[0],
                        contentType,
                        new ExchangeDescription(exchange.Name, GetLinkExchangeType(exchange.Kind),
                            exchange.Durable, exchange.AutoDelete, exchange.Delayed, exchange.Alternate),
                        routingKey);
                    description.Events.Add(property.Name, eventDesc);
                }
                else if (property.PropertyType.IsGenericType &&
                         (property.PropertyType.GetGenericTypeDefinition() == typeof(Action<>) ||
                          property.PropertyType.GetGenericTypeDefinition() == typeof(Func<,>)))
                {
                    var endpointName = property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                       throw new SchemaMarkupException($"Not endpoint attribute found on {property.Name}");
                    
                    var requestExchange = property.GetCustomAttribute<ExchangeAttribute>();
                    if (requestExchange != null)
                    {
                        if (string.IsNullOrWhiteSpace(requestExchange.Name))
                            requestExchange.Name = $"{serviceOwner}.{serviceName}.{endpointName}";
                    }
                    
                    
                    var responseExchange = property.GetCustomAttribute<ResponseExchangeAttribute>();
                    if (responseExchange != null)
                    {
                        if (responseExchange.Name == null)
                            responseExchange.Name = $"{serviceOwner}.{serviceName}.{endpointName}.responses";
                    }
                    else
                    {
                        responseExchange = requestExchange != null && requestExchange.Kind != ExchangeKind.Fanout
                            ? new ResponseExchangeAttribute
                            {
                                Name = requestExchange.Name,
                                Alternate = requestExchange.Alternate,
                                AutoDelete = requestExchange.AutoDelete,
                                Delayed = requestExchange.Delayed,
                                Durable = requestExchange.Durable,
                                Kind = requestExchange.Kind
                            }
                            : GetDefaultResponseExchange(type);
                    }
                    if(requestExchange == null)
                        requestExchange = GetDefaultExchange(type);
                                           
                    var routingKey = property.GetCustomAttribute<RoutingKeyAttribute>()?.Key ??
                                     property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                     throw new SchemaMarkupException("Missign routing key or endpoint name");
                    var contentType = property.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      type.GetCustomAttribute<ContentTypeAttribute>()?.ContentType ??
                                      new ContentType("text/json;charset=utf-8");
                    
                    var rpcQueue = property.GetCustomAttribute<RpcQueueAttribute>() ??
                                   new RpcQueueAttribute($"{serviceOwner}.{serviceName}.{endpointName}");
                    var callDesc = new CallDescription(description, endpointName, 
                        property.PropertyType.GenericTypeArguments[0],
                        property.PropertyType.GenericTypeArguments.Length > 1 
                            ? property.PropertyType.GenericTypeArguments[1] 
                            : typeof(RpcOk),
                        contentType,
                        new ExchangeDescription(requestExchange.Name, GetLinkExchangeType(requestExchange.Kind),
                            requestExchange.Durable, requestExchange.AutoDelete, requestExchange.Delayed, requestExchange.Alternate),
                        routingKey,
                        new ExchangeDescription(responseExchange.Name, GetLinkExchangeType(responseExchange.Kind),
                            requestExchange.Durable, responseExchange.AutoDelete, responseExchange.Delayed, responseExchange.Alternate), 
                        rpcQueue.Name, rpcQueue.Durable, rpcQueue.AutoDelete);
                    description.Calls.Add(property.Name, callDesc);
                }
            }
            return description;
        }
        
        private readonly ConcurrentDictionary<Type, ServiceDescription> _cache = new ConcurrentDictionary<Type, ServiceDescription>();

        public ServiceDescription GetDescription(Type type)
        {
            return _cache.GetOrAdd(type, GenerateDescription);
        }
    }
}