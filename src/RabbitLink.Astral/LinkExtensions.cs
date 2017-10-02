using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Markup;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using FunEx.Monads;
using Microsoft.Extensions.Logging;
using RabbitLink.Astral.Markup;
using RabbitLink.Messaging;
using RabbitLink.Producer;
using RabbitLink.Serialization;
using RabbitLink.Topology;

namespace RabbitLink.Astral
{
    public static class LinkExtensions
    {
        private static readonly ConditionalWeakTable<ILink, Serialization<byte[]>> SerializationDict = new ConditionalWeakTable<ILink, Serialization<byte[]>>();
        private static readonly ConditionalWeakTable<ILink, TypeEncoding> TypeEncodingDict = new ConditionalWeakTable<ILink, TypeEncoding>();
        private static readonly ConditionalWeakTable<ILink, ILoggerFactory> LoggerFactoryDict = new ConditionalWeakTable<ILink, ILoggerFactory>();

        private static readonly ConditionalWeakTable<ILink, ConcurrentDictionary<(string, bool), ILinkProducer>>
            Producers = new ConditionalWeakTable<ILink, ConcurrentDictionary<(string, bool), ILinkProducer>>();
            
        
        public static ILink Serialization(this ILink link, Serialization<byte[]> serialization)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));
            if (serialization == null) throw new ArgumentNullException(nameof(serialization));
            SerializationDict.Add(link, serialization);
            return link;
        }

        public static ILink TypeEncoding(this ILink link, TypeEncoding encoding)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            TypeEncodingDict.Add(link, encoding);
            return link;
        }

        public static ILink LoggerFactory(this ILink link, ILoggerFactory factory)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            LoggerFactoryDict.Add(link, factory);
            return link;
        }

        internal static ILoggerFactory GetLoggerFactory(this ILink link)
            => LoggerFactoryDict.GetValue(link, _ => new FakeLoggerFactory());

        internal static Serialization<byte[]> GetSerialization(this ILink link)
            => SerializationDict.GetValue(link, _ => global::Astral.Payloads.Serialization.Serialization.JsonRaw);


        internal static TypeEncoding GetTypeEncoding(this ILink link)
            => TypeEncodingDict.GetValue(link, _ => global::Astral.Payloads.DataContracts.TypeEncoding.Default);

        internal static ILinkProducer GetOrAddProducer(this ILink link, string exchange, bool confirmsMode,
            Func<ILink, ILinkProducer> factory)
            => Producers.GetOrCreateValue(link).GetOrAdd((exchange, confirmsMode), _ => factory(link));
        
        public static IServiceBuilder<TService> Service<TService>(this ILink link)
        {
            return new ServiceBuilder<TService>(link);
        }
    }

    public interface IServiceBuilder<TService>
    {
        IEventPublisher<TService, TEvent> Endpoint<TEvent>(Expression<Func<TService, EventHandler<TEvent>>> selector)
            where TEvent : class;
    }


    public interface IEventPublisher<TService, TEvent>
        where TEvent : class 
    {
        Task PublishAsync(TEvent @event, CancellationToken token = default(CancellationToken));
    }

    internal class ServiceBuilder<TService> : IServiceBuilder<TService>
    {
        private readonly ILink _link;

        public ServiceBuilder(ILink link)
        {
            _link = link;
        }


        public IEventPublisher<TService, TEvent> Endpoint<TEvent>(Expression<Func<TService, EventHandler<TEvent>>> selector) where TEvent : class
        {
            return new EventEndpoint<TService, TEvent>(_link, selector.GetProperty(), true);
        }
    }

    internal class Endpoint<TService>
    {
        protected Endpoint(ILink link, PropertyInfo property)
        {
            Link = link;
            Property = property;
        }

        protected ILink Link { get; }
        protected PropertyInfo Property { get; }

        protected ExchangeAttribute GetDefaultExchange()
        {
            var owner = typeof(TService).GetCustomAttribute<OwnerAttribute>()?.OwnerName ??
                        throw new AstralException("Owner not specified");
            var serviceName = typeof(TService).GetCustomAttribute<ServiceAttribute>()?.Name ??
                              throw new AstralException("Service name not specified");
            return new ExchangeAttribute($"{owner}.{serviceName}");
        }
    }

    internal class EventEndpoint<TService, TEvent> : Endpoint<TService>, IEventPublisher<TService, TEvent>
        where TEvent : class
    {
        private readonly bool _confirmsMode;
        private readonly Lazy<ILinkProducer> _linkProducer;
        private readonly bool _passive;
        
        
        
        public EventEndpoint(ILink link, PropertyInfo property, bool confirmsMode) : base(link, property)
        {
            _confirmsMode = confirmsMode;
            _linkProducer = new Lazy<ILinkProducer>(MakeProducer);
        }

        private ILinkProducer MakeProducer()
        {
            var exchange = Property.GetCustomAttribute<ExchangeAttribute>() ??
                           typeof(TService).GetCustomAttribute<ExchangeAttribute>() ?? GetDefaultExchange();
            string routingKey = null;
            if (exchange.Kind != ExchangeKind.Fanout)
                routingKey = Property.GetCustomAttribute<RoutingKeyAttribute>()?.Key ??
                             Property.GetCustomAttribute<EndpointAttribute>()?.Name ??
                             throw new AstralException("Missign routing key or endpoint name");
            return Link.GetOrAddProducer(exchange.Name, _confirmsMode, _ =>
                Link
                    .Producer
                    .Exchange(cfg => _passive
                        ? cfg.ExchangeDeclarePassive(exchange.Name)
                        : cfg.ExchangeDeclare(exchange.Name,
                            exchange.Kind == ExchangeKind.Fanout ? LinkExchangeType.Fanout :
                            exchange.Kind == ExchangeKind.Direct ? LinkExchangeType.Direct : LinkExchangeType.Topic))
                    .ConfirmsMode(_confirmsMode)
                    .PublishProperties(new LinkPublishProperties
                    {
                        RoutingKey = routingKey
                    })
                    .Serializer(new PayloadLink(Link.GetSerialization(), Link.GetTypeEncoding(),
                        new ContentType("text/json;encoding=utf8"),
                        Link.GetLoggerFactory().CreateLogger<PayloadLink>()))
                    .Build());
        }


        public Task PublishAsync(TEvent @event, CancellationToken token)
        {
            return _linkProducer.Value.PublishAsync(new LinkPublishMessage<TEvent>(@event), token);
        }
    }

    internal class PayloadLink : ILinkSerializer
    {
        private readonly Serialization<byte[]> _serialization;
        private readonly TypeEncoding _encoding;
        private readonly ContentType _contentType;
        private readonly ILogger _logger;

        public PayloadLink(Serialization<byte[]> serialization, TypeEncoding encoding, ContentType contentType, ILogger logger)
        {
            _serialization = serialization;
            _encoding = encoding;
            _contentType = contentType;
            _logger = logger;
        }

        public byte[] Serialize<TBody>(TBody body, LinkMessageProperties properties) where TBody : class
        {
            var serialized = Payload.ToPayload(_logger, body,
                new PayloadEncode<byte[]>(_contentType, _encoding.Encode, _serialization.Serialize)).Unwrap();
            properties.Type = serialized.TypeCode;
            properties.ContentType = serialized.ContentType.ToString();
            return serialized.Data;
        }

        public TBody Deserialize<TBody>(byte[] body, LinkMessageProperties properties) where TBody : class
        {
            throw new NotImplementedException();
        }
    }
    
}