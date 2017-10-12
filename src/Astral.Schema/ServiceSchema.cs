using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.Green;

namespace Astral.Schema
{

    public class ServiceSchema
    {
        private readonly ImmutableDictionary<string, Lazy<CallSchema>> _lazyCallByName;
        private readonly ImmutableDictionary<string, string> _callNameToCodeName;
        private readonly Lazy<ExchangeSchema> _lazyExchange;
        private readonly Lazy<ExchangeSchema> _lazyResponseExchange;
        
        public ServiceSchema(ServiceSchemaGreen greenSchema)
        {
            Green = greenSchema;
            var builder = ImmutableDictionary.CreateBuilder<string, Lazy<CallSchema>>();
            builder.AddRange(greenSchema.Calls.Select(
                p => new KeyValuePair<string, Lazy<CallSchema>>(p.Key,
                    new Lazy<CallSchema>(() => new CallSchema(this, p.Value)))));
            _lazyCallByName = builder.ToImmutable();
            var nameBuilder = ImmutableDictionary.CreateBuilder<string, string>();
            nameBuilder.AddRange(greenSchema.Calls.Select(p => new KeyValuePair<string, string>(p.Value.CodeName, p.Key)));
            _callNameToCodeName = nameBuilder.ToImmutable();
            _lazyExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = Green.Exchange;
                if (exchange == null) 
                    return new ExchangeSchema($"{Green.Owner}.{Green.Name}".ToLowerInvariant());
                if(string.IsNullOrWhiteSpace(exchange.Name))
                    return new ExchangeSchema($"{Green.Owner}.{Green.Name}".ToLowerInvariant(), exchange.Type,
                        exchange.Durable, exchange.AutoDelete,
                        exchange.Delayed, exchange.Alternate);
                return exchange;
            });
            
            _lazyResponseExchange = new Lazy<ExchangeSchema>(() =>
            {
                var exchange = Green.ResponseExchange;
                if (exchange == null)
                    return _lazyExchange.Value;
                
                if(exchange.Name == null)
                    return new ExchangeSchema($"{Green.Owner}.{Green.Name}.responses".ToLowerInvariant(), exchange.Type,
                        exchange.Durable, exchange.AutoDelete,
                        exchange.Delayed, exchange.Alternate);
                return exchange;
            });
        }

        internal ServiceSchemaGreen Green { get; }
        
        
        public ContentType ContentType => Green.ContentType ?? new ContentType("text/json;charset=utf-8");
        public bool HasContentType => Green.ContentType != null;

        public CallSchema CallByName(string name) => _lazyCallByName[name].Value;

        public bool TryCallByName(string name, out CallSchema callSchema)
        {
            if (_lazyCallByName.TryGetValue(name, out var value))
            {
                callSchema = value.Value;
                return true;
            }
            callSchema = null;
            return false;
        }

        public bool ContainsCallName(string name) => _lazyCallByName.ContainsKey(name);
        
        public CallSchema CallByCodeName(string codeName) => _lazyCallByName[_callNameToCodeName[codeName]].Value;

        public bool TryCallByCodeName(string codeName, out CallSchema callSchema)
        {
            if (_callNameToCodeName.TryGetValue(codeName, out var name))
            {
                callSchema = _lazyCallByName[name].Value;
                return true;
            }
            callSchema = null;
            return false;
        }

        public bool ContainsCallCodeName(string codeName) => _callNameToCodeName.ContainsKey(codeName);

        
        public ExchangeSchema Exchange => _lazyExchange.Value;
        public bool HasExchange => Green.Exchange != null;
        
        public ExchangeSchema ResponseExchange => _lazyResponseExchange.Value;
        public bool HasResponseExchange => Green.ResponseExchange != null;

        public string Name => Green.Name;
        public string Owner => Green.Owner;

        public string CodeName => Green.CodeName;

        public ServiceSchema SetCodeName(string value)
        {
            return new ServiceSchema(new ServiceSchemaGreen(Green.Name, Green.Owner, value, Green.Events, Green.Calls, Green.Types,
                Green.ContentType, Green.Exchange, Green.ResponseExchange));
        }
        
        public ITypeDeclarationSchema TypeById(int id) => throw new NotImplementedException();

        /*
        public static ServiceSchema FromType<T>(bool convertNames = false)
        {
            var serviceType = typeof(T);
            return FromType(serviceType, convertNames);
        }

        public static ServiceSchema FromType(Type serviceType, bool convertNames)
        {
            if (!serviceType.IsInterface)
                throw new SchemaException($"Type of service must be interface");
            var owner = serviceType.GetCustomAttribute<OwnerAttribute>()?.OwnerName ??
                        (convertNames
                            ? OwnerFromNamespace(serviceType.Namespace)
                            : throw new SchemaException($"Type {serviceType} has no OwnerAttribute"));
            var name = serviceType.GetCustomAttribute<ServiceAttribute>()?.Name ??
                       (convertNames
                           ? ServiceNameFromInterfaceName(serviceType.Name)
                           : throw new SchemaException($"Type {serviceType} has no ServiceAttribute"));

            var schema = new RootSchema(name, owner)
                .ServiceType(serviceType)
                .CodeName(serviceType.Name);

            var contentTypeAttr = serviceType.GetCustomAttribute<ContentTypeAttribute>();
            if (contentTypeAttr != null)
                schema = schema.ContentType(contentTypeAttr.ContentType);

            var exchangeAttr = serviceType.GetCustomAttribute<ExchangeAttribute>();
            if (exchangeAttr != null)
            {
                schema = schema.Exchange(new ExchangeSchema(exchangeAttr.Name, exchangeAttr.Kind, exchangeAttr.Durable,
                    exchangeAttr.AutoDelete,
                    exchangeAttr.Delayed, exchangeAttr.Alternate));
            }
            var responseExchangeAttr = serviceType.GetCustomAttribute<ResponseExchangeAttribute>();
            if (responseExchangeAttr != null)
            {
                schema = schema.ResponseExchange(new ExchangeSchema(responseExchangeAttr.Name,
                    responseExchangeAttr.Kind, responseExchangeAttr.Durable,
                    responseExchangeAttr.AutoDelete, responseExchangeAttr.Delayed, responseExchangeAttr.Alternate));
            }

            var events = new List<EventSchema>();
            var calls = new List<CallSchema>();
            var types = new List<(Type[], Action<string[]>)>();

            foreach (var property in serviceType.GetProperties())
            {
                if (property.PropertyType.IsConstructedGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(EventHandler<>))
                {
                    var endpointName = property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                       (convertNames
                                           ? PropertyNameToEndpointName(property.Name)
                                           : throw new SchemaException(
                                               $"Service {serviceType}, event {property.Name} hs no EndpointAttribute"));
                    var endpoint = new EventSchema(schema, endpointName)
                        .ContractType(property.PropertyType.GenericTypeArguments[0])
                        .CodeName(property.Name);
                    contentTypeAttr = property.GetCustomAttribute<ContentTypeAttribute>();
                    if (contentTypeAttr != null)
                        endpoint = endpoint.ContentType(contentTypeAttr.ContentType);
                    exchangeAttr = property.GetCustomAttribute<ExchangeAttribute>();
                    if (exchangeAttr != null)
                        endpoint = endpoint.Exchange(new ExchangeSchema(exchangeAttr.Name, exchangeAttr.Kind,
                            exchangeAttr.Durable,
                            exchangeAttr.AutoDelete,
                            exchangeAttr.Delayed, exchangeAttr.Alternate));
                    types.Add((new [] {endpoint.ContractType() }, p =>
                    {
                        var ep = endpoint.ContractName(p[0]);
                        events.Add(ep);
                    }));
                }
                else if (property.PropertyType.IsConstructedGenericType &&
                         (property.PropertyType.GetGenericTypeDefinition() == typeof(Action<>) ||
                          property.PropertyType.GetGenericTypeDefinition() == typeof(Func<,>)))
                {
                    var endpointName = property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                                       (convertNames
                                           ? PropertyNameToEndpointName(property.Name)
                                           : throw new SchemaException(
                                               $"Service {serviceType}, event {property.Name} hs no EndpointAttribute"));
                    var endpoint = new CallSchema(schema, endpointName)
                        .RequestType(property.PropertyType.GenericTypeArguments[0])
                        .ResponseType(property.PropertyType.GenericTypeArguments.Length > 1
                            ? property.PropertyType.GenericTypeArguments[1]
                            : null)
                        .CodeName(property.Name);
                    contentTypeAttr = property.GetCustomAttribute<ContentTypeAttribute>();
                    if (contentTypeAttr != null)
                        endpoint = endpoint.ContentType(contentTypeAttr.ContentType);
                    exchangeAttr = property.GetCustomAttribute<ExchangeAttribute>();
                    if (exchangeAttr != null)
                        endpoint = endpoint.Exchange(new ExchangeSchema(exchangeAttr.Name, exchangeAttr.Kind,
                            exchangeAttr.Durable,
                            exchangeAttr.AutoDelete,
                            exchangeAttr.Delayed, exchangeAttr.Alternate));
                    responseExchangeAttr = property.GetCustomAttribute<ResponseExchangeAttribute>();
                    if (responseExchangeAttr != null)
                    {
                        endpoint = endpoint.ResponseExchange(new ExchangeSchema(responseExchangeAttr.Name,
                            responseExchangeAttr.Kind, responseExchangeAttr.Durable,
                            responseExchangeAttr.AutoDelete, responseExchangeAttr.Delayed, responseExchangeAttr.Alternate));
                    }
                    var responseQueueAttr = property.GetCustomAttribute<RpcQueueAttribute>();
                    if (responseQueueAttr != null)
                        endpoint = endpoint.RequestQueue(new RequestQueueSchema(responseQueueAttr.Name,
                            responseQueueAttr.Durable, responseQueueAttr.AutoDelete));
                    
                    if(property.PropertyType.GenericTypeArguments.Length > 1)
                        types.Add((
                            new [] {property.PropertyType.GenericTypeArguments[0], property.PropertyType.GenericTypeArguments[1]},
                            m =>
                            {
                                var ep = endpoint.RequestContract(m[0]).ResponseContract(m[1]);
                                calls.Add(ep);
                            }));
                }
            }
            var typeDescs = SchemaMaker.FromTypeList(types, false).ToList();
            return new ServiceSchema(schema, events, calls, typeDescs);
        }

        public static string PropertyNameToEndpointName(string propertyName)
            => propertyName.SelectMany((p, i) => char.IsUpper(p) && i > 0 ? new[] {'.', p} : new[] {p})
                .Aggregate(new StringBuilder(), (sb, ch) => sb.Append(ch), sb => sb.ToString());
        
        private static string ServiceNameFromInterfaceName(string name)
        {
            var n1 = name.Substring(1);
            return n1.SelectMany((p, i) => char.IsUpper(p) && i > 0 ? new[] {'.', char.ToLower(p)} : new[] {p})
                .Aggregate(new StringBuilder(), (sb, ch) => sb.Append(ch), sb => sb.ToString());
        }

        private static string OwnerFromNamespace(string nameSpace)
        {
            var pos = nameSpace.LastIndexOf(".", StringComparison.InvariantCulture);
            if (pos < 0)
                return nameSpace.ToLower();
            var lastPart = nameSpace.Substring(pos + 1);
            return lastPart.ToLower();
        }
        */

    }
}