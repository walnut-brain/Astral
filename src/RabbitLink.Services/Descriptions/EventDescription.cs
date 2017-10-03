using System;
using System.Net.Mime;
using RabbitLink.Topology;

namespace RabbitLink.Services.Descriptions
{
    public class EventDescription
    {
        public EventDescription(ServiceDescription service, string name, ExchangeDescription exchange, Func<object, string> routingKeyExtractor, Type type, ContentType contentType)
        {
            Exchange = exchange;
            RoutingKeyExtractor = routingKeyExtractor;
            Type = type;
            ContentType = contentType;
            Service = service;
            Name = name;
        }

        public EventDescription(ServiceDescription service, string name, ExchangeDescription exchange, string routingKey, Type type, ContentType contentType)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Type = type;
            ContentType = contentType;
            Service = service;
            Name = name;
        }


        public ExchangeDescription Exchange { get; }
        public string RoutingKey { get; }
        public Func<object, string> RoutingKeyExtractor { get; }
        public Type Type { get; }
        public ContentType ContentType { get; }
        public string Name { get; }
        public ServiceDescription Service { get; }
    }
}