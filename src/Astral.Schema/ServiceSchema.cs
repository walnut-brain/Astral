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

    public class ServiceSchema : IServiceSchema
    {
        private readonly ImmutableDictionary<string, Lazy<CallSchema>> _lazyCallByName;
        private readonly ImmutableDictionary<string, string> _callNameByCodeName;
        private readonly ImmutableDictionary<string, Lazy<EventSchema>> _lazyEventByName;
        private readonly ImmutableDictionary<string, string> _eventNameByCodeName;
        private readonly ImmutableDictionary<int, Lazy<ITypeSchema>> _lazyTypes;
        private readonly Lazy<ExchangeSchema> _lazyExchange;
        private readonly Lazy<ExchangeSchema> _lazyResponseExchange;
        
        public ServiceSchema(ServiceSchemaGreen greenSchema)
        {
            Green = greenSchema;
            _lazyCallByName = ImmutableDictionary.CreateRange(greenSchema.Calls.Select(
                p => new KeyValuePair<string, Lazy<CallSchema>>(p.Key,
                    new Lazy<CallSchema>(() => new CallSchema(this, p.Value)))));
             
            _callNameByCodeName = ImmutableDictionary.CreateRange(greenSchema.Calls.Select(p => new KeyValuePair<string, string>(p.Value.CodeName, p.Key)));
             
            _lazyEventByName = ImmutableDictionary.CreateRange(greenSchema.Events.Select(
                p => new KeyValuePair<string, Lazy<EventSchema>>(p.Key,
                    new Lazy<EventSchema>(() => new EventSchema(this, p.Value)))));
            _eventNameByCodeName = ImmutableDictionary.CreateRange(greenSchema.Events.Select(p => new KeyValuePair<string, string>(p.Value.CodeName, p.Key)));

            _lazyTypes = ImmutableDictionary.CreateRange(greenSchema.Types.Select(p =>
                new KeyValuePair<int, Lazy<ITypeSchema>>(
                    p.Key, new Lazy<ITypeSchema>(() =>
                    {
                        switch (p.Value)
                        {
                            case ArrayTypeSchemaGreen arrayTypeSchemaGreen:
                                return new ArrayTypeSchema(this, arrayTypeSchemaGreen);
                            case ComplexTypeSchemaGreen complexTypeSchemaGreen:
                                return new ComplexTypeSchema(this, complexTypeSchemaGreen);
                            case EnumTypeSchemaGreen enumTypeSchemaGreen:
                                return new EnumTypeSchema(this, enumTypeSchemaGreen);
                            case NullableTypeSchemaGreen nullableTypeSchemaGreen:
                                return new NullableTypeSchema(this, nullableTypeSchemaGreen);
                            case WellKnownTypeSchemaGreen wellKnownTypeSchemaGreen:
                                return new WellKnownTypeSchema(wellKnownTypeSchemaGreen);
                            default:
                                throw new ArgumentOutOfRangeException(
                                    $"Unknown green type name schema {p.Value?.GetType()}");
                        }
                    }))));
            
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
            
            
            _eventsLazy = new Lazy<IReadOnlyCollection<IEventSchema>>(() => _lazyEventByName.Values.Select(p => p.Value).ToImmutableList());

            _callsLazy = new Lazy<IReadOnlyCollection<ICallSchema>>(() => _lazyCallByName.Values.Select(p => p.Value).ToImmutableList());

            _typesLazy = new Lazy<IReadOnlyCollection<ITypeSchema>>( () => _lazyTypes.Values.Select(p => p.Value).ToImmutableList());
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
        
        public CallSchema CallByCodeName(string codeName) => _lazyCallByName[_callNameByCodeName[codeName]].Value;

        public bool TryCallByCodeName(string codeName, out CallSchema callSchema)
        {
            if (_callNameByCodeName.TryGetValue(codeName, out var name))
            {
                callSchema = _lazyCallByName[name].Value;
                return true;
            }
            callSchema = null;
            return false;
        }
        
        public EventSchema EventByName(string name) => _lazyEventByName[name].Value;

        public bool TryEventByName(string name, out EventSchema eventSchema)
        {
            if (_lazyEventByName.TryGetValue(name, out var value))
            {
                eventSchema = value.Value;
                return true;
            }
            eventSchema = null;
            return false;
        }

        public bool ContainsEventName(string name) => _lazyEventByName.ContainsKey(name);
        
        public EventSchema EventByCodeName(string codeName) => _lazyEventByName[_eventNameByCodeName[codeName]].Value;

        public bool TryEventByCodeName(string codeName, out EventSchema eventSchema)
        {
            if (_eventNameByCodeName.TryGetValue(codeName, out var name))
            {
                eventSchema = _lazyEventByName[name].Value;
                return true;
            }
            eventSchema = null;
            return false;
        }

        public bool ContainsEventCodeName(string codeName) => _eventNameByCodeName.ContainsKey(codeName);

        public bool ContainsCallCodeName(string codeName) => _callNameByCodeName.ContainsKey(codeName);

        
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
        
        public ITypeSchema TypeById(int id) => _lazyTypes[id].Value;

        
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

            var serviceExchangeAttr = serviceType.GetCustomAttribute<ExchangeAttribute>();
            var serviceExchange = serviceExchangeAttr != null
                ? new ExchangeSchema(serviceExchangeAttr.Name, serviceExchangeAttr.Kind, serviceExchangeAttr.Durable,
                    serviceExchangeAttr.AutoDelete,
                    serviceExchangeAttr.Delayed, serviceExchangeAttr.Alternate)
                : null;
            var sResponseExchangeAttr = serviceType.GetCustomAttribute<ResponseExchangeAttribute>();
            var serviceResponseExchange = sResponseExchangeAttr != null
                ? new ExchangeSchema(sResponseExchangeAttr.Name,
                    sResponseExchangeAttr.Kind, sResponseExchangeAttr.Durable,
                    sResponseExchangeAttr.AutoDelete, sResponseExchangeAttr.Delayed, sResponseExchangeAttr.Alternate)
                : null;
        

            var events = new List<EventSchemaGreen>();
            var calls = new List<CallSchemaGreen>();
            var types = new List<(Type[], Action<int[]>)>();

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
                    var type = property.PropertyType.GenericTypeArguments[0];
                    var prop = property;
                    var exchangeAttr = property.GetCustomAttribute<ExchangeAttribute>();
                    var exchange = exchangeAttr != null
                        ? new ExchangeSchema(exchangeAttr.Name, exchangeAttr.Kind,
                            exchangeAttr.Durable,
                            exchangeAttr.AutoDelete,
                            exchangeAttr.Delayed, exchangeAttr.Alternate)
                        : null;
                    types.Add((new [] {type}, p =>
                    {
                        events.Add(new EventSchemaGreen(endpointName, prop.Name, p[0], 
                            prop.GetCustomAttribute<ContentTypeAttribute>()?.ContentType,
                            prop.GetCustomAttribute<RoutingKeyAttribute>()?.Key,
                            exchange));
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
                    
                    var requestType = property.PropertyType.GenericTypeArguments[0];
                    var responseType = property.PropertyType.GenericTypeArguments.Length > 1
                            ? property.PropertyType.GenericTypeArguments[1]
                            : null;
                    var exchangeAttr = property.GetCustomAttribute<ExchangeAttribute>();
                    var exchange = exchangeAttr != null
                        ? new ExchangeSchema(exchangeAttr.Name, exchangeAttr.Kind,
                            exchangeAttr.Durable,
                            exchangeAttr.AutoDelete,
                            exchangeAttr.Delayed, exchangeAttr.Alternate)
                        : null;
                    var responseExchangeAttr = property.GetCustomAttribute<ResponseExchangeAttribute>();
                    var responseExchange = responseExchangeAttr != null
                        ? new ExchangeSchema(responseExchangeAttr.Name,
                            responseExchangeAttr.Kind, responseExchangeAttr.Durable,
                            responseExchangeAttr.AutoDelete, responseExchangeAttr.Delayed,
                            responseExchangeAttr.Alternate)
                        : null;
                    var responseQueueAttr = property.GetCustomAttribute<RpcQueueAttribute>();
                    var responseQueue = responseQueueAttr != null
                        ? new RequestQueueSchema(responseQueueAttr.Name,
                            responseQueueAttr.Durable, responseQueueAttr.AutoDelete)
                        : null;
                    var prop = property;
                    if(responseType != null)
                        types.Add((
                            new [] {requestType, responseType},
                            m =>
                            {
                                calls.Add(new CallSchemaGreen(endpointName, prop.Name, m[0], m[1], 
                                    prop.GetCustomAttribute<ContentTypeAttribute>()?.ContentType,
                                    prop.GetCustomAttribute<RoutingKeyAttribute>()?.Key,
                                    responseQueue, exchange, responseExchange));
                            }));
                    else
                        types.Add((
                            new [] {requestType},
                            m =>
                            {
                                calls.Add(new CallSchemaGreen(endpointName, prop.Name, m[0], null, 
                                    prop.GetCustomAttribute<ContentTypeAttribute>()?.ContentType,
                                    prop.GetCustomAttribute<RoutingKeyAttribute>()?.Key,
                                    responseQueue, exchange, responseExchange));
                            }));
                }
            }
            var typeDescs = SchemaMaker.FromTypeList(types, false);
            return new ServiceSchema(new ServiceSchemaGreen(name, owner, serviceType.Name, 
                ImmutableDictionary.CreateRange(events.Select(p => new KeyValuePair<string, EventSchemaGreen>(p.Name, p))), 
                ImmutableDictionary.CreateRange(calls.Select(p => new KeyValuePair<string, CallSchemaGreen>(p.Name, p))),
                typeDescs, serviceType.GetCustomAttribute<ContentTypeAttribute>()?.ContentType,
                serviceExchange, serviceResponseExchange));
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

        public IEnumerable<EventSchema> Events => _lazyEventByName.Values.Select(p => p.Value);
        public IEnumerable<CallSchema> Calls => _lazyCallByName.Values.Select(p => p.Value);
        public IEnumerable<ITypeSchema> Types => _lazyTypes.Values.Select(p => p.Value);


        private readonly Lazy<IReadOnlyCollection<IEventSchema>> _eventsLazy;
        IReadOnlyCollection<IEventSchema> IServiceSchema.Events => _eventsLazy.Value;

        private readonly Lazy<IReadOnlyCollection<ICallSchema>> _callsLazy;
        IReadOnlyCollection<ICallSchema> IServiceSchema.Calls => _callsLazy.Value;


        private readonly Lazy<IReadOnlyCollection<ITypeSchema>> _typesLazy;
        IReadOnlyCollection<ITypeSchema> IServiceSchema.Types => _typesLazy.Value;


        IEventSchema IServiceSchema.EventByCodeName(string codeName) => EventByCodeName(codeName);

        ICallSchema IServiceSchema.CallByCodeName(string codeName) => CallByCodeName(codeName);
        
    }
}