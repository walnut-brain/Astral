using System;
using System.Collections.Immutable;
using System.Net.Mime;
using Astral.Markup.RabbitMq;

namespace Astral.Schema.Green
{
    public class ServiceSchemaGreen 
    {
        public ServiceSchemaGreen(string name, string owner, string codeName, ImmutableDictionary<string, EventSchemaGreen> events,
            ImmutableDictionary<string, CallSchemaGreen> calls, ImmutableDictionary<int, TypeSchemaGreen> types, ContentType contentType = null,
            ExchangeSchema exchange = null, ExchangeSchema responseExchange = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(owner));
            Name = name;
            Owner = owner;
            CodeName = codeName;
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Calls = calls ?? throw new ArgumentNullException(nameof(calls));
            ContentType = contentType;
            if(exchange != null && exchange.Type == ExchangeKind.Fanout)
                throw new SchemaException($"Invalid exchange type on service level in service {Name} - cannot be Fanout");
            Exchange = exchange;
            ResponseExchange = responseExchange;
            if(responseExchange != null && responseExchange.Type == ExchangeKind.Fanout)
                throw new SchemaException($"Invalid response exchange type on service level in service {Name} - cannot be Fanout");
            if (string.IsNullOrWhiteSpace(codeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codeName));
            Types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public string Name { get; }
        public string Owner { get; }
        public string CodeName { get; }
        public ExchangeSchema Exchange { get; }
        public ExchangeSchema ResponseExchange { get; }
        public ImmutableDictionary<string, EventSchemaGreen> Events { get; }
        public ImmutableDictionary<string, CallSchemaGreen> Calls { get; }
        public ImmutableDictionary<int, TypeSchemaGreen> Types { get; }
        public ContentType ContentType { get; }
    }

    
}