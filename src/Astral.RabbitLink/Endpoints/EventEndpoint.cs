using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using RabbitLink.Consumer;
using RabbitLink.Messaging;
using RabbitLink.Topology;

namespace Astral.RabbitLink
{
    internal class EventEndpoint<TService, TEvent> : Endpoint<EventDescription>, IEventEndpoint<TService, TEvent> 
    {
        public EventEndpoint(ServiceLink link, EventDescription description)
            : base(link, description)
        {
        }

        private EventEndpoint(ServiceLink link, EventDescription description, IReadOnlyDictionary<string, object> store) 
            : base(link, description, store)
        {
            
        }

        public bool ExchangePassive() => GetParameter(nameof(ExchangePassive), false);
        public IEventEndpoint<TService, TEvent> ExchangePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(ExchangePassive), value));
        
        public string NamedProducer() => GetParameter(nameof(NamedProducer), (string) null);
        public IEventEndpoint<TService, TEvent> NamedProducer(string value) 
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(NamedProducer), value));

        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IEventEndpoint<TService, TEvent> ConfirmsMode(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(ConfirmsMode), value));

        public string QueueName() => GetParameter(nameof(QueueName),
            $"{Link.HolderName}.{Description.Service.Owner}.{Description.Service.Name}.{Description.Name}");
        public IEventEndpoint<TService, TEvent> QueueName(string value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(QueueName), value));

        public ushort PrefetchCount() => GetParameter<ushort>(nameof(PrefetchCount), 1);
        public IEventEndpoint<TService, TEvent> PrefetchCount(ushort value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(PrefetchCount), value));


        public bool AutoAck() => GetParameter(nameof(AutoAck), false);
        public IEventEndpoint<TService, TEvent> AutoAck(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetParameter(nameof(ErrorStrategy), (ILinkConsumerErrorStrategy) null);
        public IEventEndpoint<TService, TEvent> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetParameter(nameof(CancelOnHaFailover), (bool?) null);
        public IEventEndpoint<TService, TEvent> CancelOnHaFailover(bool? value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        public IEventEndpoint<TService, TEvent> Exclusive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(Exclusive), value));

        public bool QueuePassive() => GetParameter(nameof(QueuePassive), false);
        public IEventEndpoint<TService, TEvent> QueuePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(QueuePassive), value));
        
        public bool Bind() => GetParameter(nameof(Bind), true);
        public IEventEndpoint<TService, TEvent> Bind(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetParameter(nameof(QueueParameters), new QueueParameters());
        public IEventEndpoint<TService, TEvent> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new EventEndpoint<TService, TEvent>(Link, Description, SetParameter(nameof(QueueParameters), setter(QueueParameters())));


        public IEnumerable<string> RoutingKeys() => GetParameter(nameof(RoutingKeys), Enumerable.Empty<string>());
        public IEventEndpoint<TService, TEvent> AddRoutingKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var lst = RoutingKeys().ToList();
            lst.Add(value);
            return new EventEndpoint<TService, TEvent>(Link,
                Description, SetParameter<IEnumerable<string>>(nameof(RoutingKeys), new ReadOnlyCollection<string>(lst)));
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
                ConfirmsMode(), NamedProducer());
            return publisher.PublishAsync(msg, token);
        }
    }
}