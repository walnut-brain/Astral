using System;
using System.Collections.Generic;
using System.Net.Mime;
using Astral.Schema.Green;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public class EventSchema : IRabbitMqEventSchema 
    {
        private readonly EventSchemaGreen _green;
        private readonly Lazy<ExchangeSchema> _lazyExchange;
        private readonly Lazy<string> _routingKey;
        
        public ServiceSchema Service { get; }

        IServiceSchema IEndpointSchema.Service => Service;
        

        public EventSchema(ServiceSchema service, EventSchemaGreen green)
        {
            _green = green;
            Service = service;
            _lazyExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = _green.Exchange;
                if (exchange == null)
                    return Service.Exchange;
                if(string.IsNullOrWhiteSpace(exchange.Name))
                    return new ExchangeSchema($"{service.Owner}.{service.Name}.{Name}".ToLower(),
                        exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Delayed, exchange.Alternate);
                return exchange;
            });
            _routingKey = new Lazy<string>(() =>
            {
                var key = _green.RoutingKey;
                if (key == null)
                    return Name.ToLower();
                return key;
            });
        }

        public string Name => _green.Name;
        public string CodeName => _green.CodeName;

        public ServiceSchema SetCodeName(string codeName)
        {
            var newEvents = Service.Green.Events.SetItem(Name, new EventSchemaGreen(Name, codeName, _green.TypeId,
                _green.ContentType, _green.RoutingKey, _green.Exchange));
            return new ServiceSchema(new ServiceSchemaGreen(Service.Green.Name, Service.Green.Owner, Service.Green.CodeName,
                newEvents, Service.Green.Calls, Service.Green.Types, Service.Green.ContentType, Service.Green.Exchange, Service.Green.ResponseExchange));
        }

        public ContentType ContentType => _green.ContentType ?? Service.ContentType;
        public bool HasContentType => _green.ContentType != null;

        public ExchangeSchema Exchange => _lazyExchange.Value;
        public bool HasExchange => _green.Exchange != null;

        IExchangeSchema IRabbitMqEndpointSchema.Exchange => Exchange;
        

        public string RoutingKey => _routingKey.Value;
        public bool HasRoutingKey => _green.RoutingKey != null;

        public ITypeDeclarationSchema EventType => Service.TypeById(_green.TypeId);
        
        


    }
}