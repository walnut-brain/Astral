using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Astral.Markup.RabbitMq;

namespace Astral.Schema.Green
{
    public abstract class EndpointSchemaGreen : GreenNode
    {
        protected EndpointSchemaGreen(string name, string codeName, ContentType contentType = null, string routingKey = null, ExchangeSchema exchange = null)
        {
            if (string.IsNullOrWhiteSpace(codeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codeName));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            Name = name;
            ContentType = contentType;
            if(routingKey != null && string.IsNullOrWhiteSpace(routingKey))
                throw new ArgumentException("Routing key cannot be whitespace.", nameof(routingKey));
            RoutingKey = routingKey;
            Exchange = exchange;
            CodeName = codeName;
        }

        protected EndpointSchemaGreen(EndpointSchemaGreen @base, string name, string codeName, ContentType contentType = null, string routingKey = null, ExchangeSchema exchange = null) : base(@base)
        {
            Name = name;
            ContentType = contentType;
            RoutingKey = routingKey;
            Exchange = exchange;
            CodeName = codeName;
        }

        public string Name { get; }
        public ContentType ContentType { get; }
        public string RoutingKey { get; }
        public ExchangeSchema Exchange { get; }
        public string CodeName { get; }
    }

}