using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Astral.Links;
using RabbitLink.Consumer;
using RabbitLink.Messaging;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Exceptions;
using RabbitLink.Services.Internals;
using RabbitLink.Topology;

namespace RabbitLink.Services
{

    internal class EventEndpoint<TService, TEvent> : BuilderBase, IEventEndpoint<TService, TEvent> 
    {
        private EventDescription Description { get; }
        private ServiceLink Link { get; }

        public EventEndpoint(EventDescription description, ServiceLink link)
        {
            Description = description;
            Link = link;
        }

        private EventEndpoint(EventDescription description, ServiceLink link, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }

        public ContentType ContentType => Description.ContentType;
        public bool ExchangePassive() => GetValue(nameof(ExchangePassive), false);
        public IEventEndpoint<TService, TEvent> ExchangePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(ExchangePassive), value));
        
        public string ExchangeNamed() => GetValue(nameof(ExchangeNamed), (string) null);
        public IEventEndpoint<TService, TEvent> ExchangeNamed(string value) 
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(ExchangeNamed), value));

        public bool ConfirmsMode() => GetValue(nameof(ConfirmsMode), true);
        public IEventEndpoint<TService, TEvent> ConfirmsMode(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(ConfirmsMode), value));

        public string QueueName() => GetValue(nameof(QueueName),
            $"{Link.ServiceName}.{Description.Service.Owner}.{Description.Service.Name}.{Description.Name}");
        public IEventEndpoint<TService, TEvent> QueueName(string value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(QueueName), value));

        public ushort PrefetchCount() => GetValue<ushort>(nameof(PrefetchCount), 1);
        public IEventEndpoint<TService, TEvent> PrefetchCount(ushort value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(PrefetchCount), value));


        public bool AutoAck() => GetValue(nameof(AutoAck), false);
        public IEventEndpoint<TService, TEvent> AutoAck(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetValue<ILinkConsumerErrorStrategy>(nameof(ErrorStrategy), null);
        public IEventEndpoint<TService, TEvent> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetValue<bool?>(nameof(CancelOnHaFailover), null);
        public IEventEndpoint<TService, TEvent> CancelOnHaFailover(bool? value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetValue(nameof(Exclusive), false);
        public IEventEndpoint<TService, TEvent> Exclusive(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(Exclusive), value));

        public bool QueuePassive() => GetValue(nameof(QueuePassive), false);
        public IEventEndpoint<TService, TEvent> QueuePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(QueuePassive), value));
        
        public bool Bind() => GetValue(nameof(Bind), true);
        public IEventEndpoint<TService, TEvent> Bind(bool value)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetValue(nameof(QueueParameters), new QueueParameters());
        public IEventEndpoint<TService, TEvent> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new EventEndpoint<TService, TEvent>(Description, Link, SetValue(nameof(QueueParameters), setter(QueueParameters())));


        public IEnumerable<string> RoutingKeys() => GetValue(nameof(RoutingKeys), Enumerable.Empty<string>());
        public IEventEndpoint<TService, TEvent> AddRoutingKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var lst = RoutingKeys().ToList();
            lst.Add(value);
            return new EventEndpoint<TService, TEvent>(Description, Link,
                SetValue<IEnumerable<string>>(nameof(RoutingKeys), new ReadOnlyCollection<string>(lst)));
        }

        public IEventEndpoint<TService, TEvent> AddRoutingKeyByExample(TEvent value)
        {
            if(Description.RoutingKeyExtractor == null)
                throw new InvalidOperationException($"Routing key by example not supported!");
            return AddRoutingKey(Description.RoutingKeyExtractor(value));
        }

        public IDisposable Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener)
        {
            var routingKeys = new List<string>();
            if (Description.RoutingKey != null)
                routingKeys.Add(Description.RoutingKey);
            else
            {
                var t = RoutingKeys();
                if(t != null)
                    routingKeys.AddRange(t);
            }

            var consumerBuilder = Utils.CreateConsumerBuilder(Link, Description.Exchange,
                ExchangePassive(), QueuePassive(), QueueName(), AutoAck(), CancelOnHaFailover(), ErrorStrategy(),
                Exclusive(), PrefetchCount(), QueueParameters(), routingKeys, Bind());
            //consumerBuilder.Serializer(Link.PayloadManager.GetSerializer(ContentType));
            
            
            return consumerBuilder.Handler(async msg  =>
            {
                var body = (TEvent) Link.PayloadManager.Deserialize(msg, typeof(TEvent));
                switch (await listener(body, msg.Cancellation))
                {
                    case Acknowledge.Ack:
                        return LinkConsumerAckStrategy.Ack;
                    case Acknowledge.Nack:
                        return LinkConsumerAckStrategy.Nack;
                    case Acknowledge.Requeue:
                        return LinkConsumerAckStrategy.Requeue;                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).Build();
        }

        public Task PublishAsync(TEvent message, CancellationToken token = default(CancellationToken))
        {
            var props = new LinkMessageProperties();
            var serialized = Link.PayloadManager.Serialize(ContentType, message, props);
            
            var msg = new LinkPublishMessage<byte[]>(serialized, props, new LinkPublishProperties
            {
                RoutingKey =
                    Description.Exchange.Type == LinkExchangeType.Fanout ? null :
                        Description.RoutingKey ?? Description.RoutingKeyExtractor(message)
            });
            var publisher = Utils.CreateProducer(Link, Description.Exchange, Description.ContentType, ExchangePassive(),
                ConfirmsMode(), ExchangeNamed());
            return publisher.PublishAsync(msg, token);
        }
    }
}