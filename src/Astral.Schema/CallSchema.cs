using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Astral.Schema.Green;

namespace Astral.Schema
{
    public class CallSchema  
    {
        private readonly CallSchemaGreen _green;
        private readonly Lazy<ExchangeSchema> _lazyExchange;
        private readonly Lazy<ExchangeSchema> _lazyResponseExchange;
        private readonly Lazy<string> _routingKey;

        internal CallSchema(ServiceSchema service, CallSchemaGreen green)
        {
            Service = service;
            _green = green;
            _lazyExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = _green.Exchange;
                if (exchange == null)
                    return Service.Exchange;
                if(string.IsNullOrWhiteSpace(exchange.Name))
                    return new ExchangeSchema($"{service.Owner}.{service.Name}.{Name}",
                        exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Delayed, exchange.Alternate);
                return exchange;
            });
            _lazyResponseExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = _green.ResponseExchange;
                if (exchange == null)
                    return Service.ResponseExchange;
                if(exchange.Name == null)
                    return new ExchangeSchema($"{service.Owner}.{service.Name}.{Name}.responses",
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

        public ServiceSchema Service { get; }

        public string Name => _green.Name;
        public string CodeName => _green.CodeName;

        public ServiceSchema SetCodeName(string codeName)
        {
            var newCalls = Service.Green.Calls.SetItem(Name, new CallSchemaGreen(Name, codeName, _green.RequestTypeId,
                _green.ResponseTypeId, _green.ContentType,
                _green.RoutingKey, _green.RequestQueue, _green.Exchange, _green.ResponseExchange));
            return new ServiceSchema(new ServiceSchemaGreen(Service.Green.Name, Service.Green.Owner, Service.Green.CodeName,
                Service.Green.Events, newCalls, Service.Green.Types, Service.Green.ContentType, Service.Green.Exchange, Service.Green.ResponseExchange));
        }

        public ContentType ContentType => _green.ContentType ?? Service.ContentType;
        public bool HasContentType => _green.ContentType != null;

        public ExchangeSchema Exchange => _lazyExchange.Value;

        public string RoutingKey => _routingKey.Value;
        
        


    }
}