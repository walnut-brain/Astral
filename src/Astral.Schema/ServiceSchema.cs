using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{

    public class ServiceSchema : IComplexServiceSchema
    {
        private readonly RootSchema _root;
        private readonly IReadOnlyDictionary<string, IEventSchema> _eventByCodeName;
        private readonly IReadOnlyDictionary<string, ICallSchema> _callByCodeName;

        public ServiceSchema(RootSchema root, IEnumerable<EventSchema> events, IEnumerable<CallSchema> calls, IEnumerable<TypeDesc> contracts)
        {
            _root = root;
            Events = new ReadOnlyDictionary<string, EventSchema>(events.ToDictionary(p => p.Name, p => p));
            Calls = new ReadOnlyDictionary<string, CallSchema>(calls.ToDictionary(p => p.Name, p => p));
            Contracts = new ReadOnlyCollection<TypeDesc>(contracts.ToList());
            var eventSchemata = Events.Where(p => !string.IsNullOrWhiteSpace(p.Value.CodeName())).ToDictionary(p => p.Value.CodeName(), p => p.Value);
            EventByPropertyName = new ReadOnlyDictionary<string, EventSchema>(
                eventSchemata);
            var callSchemata = Calls.Where(p => !string.IsNullOrWhiteSpace(p.Value.CodeName())).ToDictionary(p => p.Value.CodeName(), p => p.Value);
            CallByPropertyName = new ReadOnlyDictionary<string, CallSchema>(
                callSchemata);
            _eventByCodeName = new ReadOnlyDictionary<string, IEventSchema>(eventSchemata.ToDictionary(p => p.Key, p => (IEventSchema) p.Value));
            _callByCodeName = new ReadOnlyDictionary<string, ICallSchema>(callSchemata.ToDictionary(p => p.Key, p => (ICallSchema) p.Value));
        }

        bool ISchema.TryGetProperty<T>(string property, out T value)
            => ((ISchema) _root).TryGetProperty(property, out value);

        string IServiceSchema.Name => _root.Name;


        string IServiceSchema.Owner => _root.Owner;

        IEnumerable<ITypeSchema> IComplexServiceSchema.Types => Contracts;

        IReadOnlyDictionary<string, IEventSchema> IComplexServiceSchema.EventByCodeName => _eventByCodeName;


        IReadOnlyDictionary<string, ICallSchema> IComplexServiceSchema.CallByCodeName => _callByCodeName;
        


        string IServiceSchema.CodeName() => _root.CodeName();


        Type IServiceSchema.ServiceType() => _root.ServiceType();

        IEnumerable<IEventSchema> IComplexServiceSchema.Events => Events.Values;


        IEnumerable<ICallSchema> IComplexServiceSchema.Calls => Calls.Values;


        public IReadOnlyDictionary<string, EventSchema> Events { get; }

        public IReadOnlyDictionary<string, CallSchema> Calls { get; }

        public IReadOnlyDictionary<string, EventSchema> EventByPropertyName { get; }
        
        public IReadOnlyDictionary<string, CallSchema> CallByPropertyName { get; }
        

        public ICollection<TypeDesc> Contracts { get; }
        
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

        
    }
}