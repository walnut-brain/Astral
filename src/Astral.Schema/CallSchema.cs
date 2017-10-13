using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Astral.Schema.Green;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public class CallSchema : IRabbitMqCallSchema
    {
        private readonly CallSchemaGreen _green;
        private readonly Lazy<ExchangeSchema> _lazyExchange;
        private readonly Lazy<ExchangeSchema> _lazyResponseExchange;
        private readonly Lazy<string> _routingKey;
        private readonly Lazy<RequestQueueSchema> _lazyQueueSchema;

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
                    return new ExchangeSchema($"{service.Owner}.{service.Name}.{Name}".ToLower(),
                        exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Delayed, exchange.Alternate);
                return exchange;
            });
            _lazyResponseExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = _green.ResponseExchange;
                if (exchange == null)
                    return Service.ResponseExchange;
                if(exchange.Name == null)
                    return new ExchangeSchema($"{service.Owner}.{service.Name}.{Name}.responses".ToLower(),
                        exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Delayed, exchange.Alternate);
                return exchange;
            });
            _lazyQueueSchema = new Lazy<RequestQueueSchema>(() =>
            {
                var queue = _green.RequestQueue;
                if(queue == null)
                    return new RequestQueueSchema($"{Service.Green.Owner}.{Service.Green.Name}.{Name}".ToLower());
                if(string.IsNullOrWhiteSpace(queue.Name))
                    return new RequestQueueSchema($"{Service.Green.Owner}.{Service.Green.Name}.{Name}".ToLower(), queue.Durable, queue.AutoDelete);
                return queue;
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
        IServiceSchema IEndpointSchema.Service => Service;

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
        public bool HasExchange => _green.Exchange != null;

        IExchangeSchema IRabbitMqEndpointSchema.Exchange => Exchange;

        public string RoutingKey => _routingKey.Value;
        public bool HasRoutingKey => _green.RoutingKey != null;

        public ExchangeSchema ResponseExchange => _lazyResponseExchange.Value;
        public bool HasResponseExchange => _green.ResponseExchange != null;

        IExchangeSchema IRabbitMqCallSchema.ResponseExchange => ResponseExchange;

        public RequestQueueSchema RequestQueue => _lazyQueueSchema.Value;
        public bool HasRequestQueue => _green.RequestQueue != null;

        IRequestQueueSchema IRabbitMqCallSchema.RequestQueue => RequestQueue;

        public ITypeDeclarationSchema RequestType => Service.TypeById(_green.RequestTypeId);
        public ITypeDeclarationSchema ResponseType 
            => _green.ResponseTypeId == null ? null : Service.TypeById(_green.ResponseTypeId.Value);
    }
}