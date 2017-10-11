using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.Markup.RabbitMq;
using Astral.RabbitLink.Internals;
using Astral.Schema;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    internal class EventEndpoint<TService, TEvent> : Endpoint<IEventSchema>, IEventEndpoint<TService, TEvent> 
    {
        public EventEndpoint(ServiceLink link, IEventSchema schema)
            : base(link, schema)
        {
        }

        private EventEndpoint(ServiceLink link, IEventSchema schema, IReadOnlyDictionary<string, object> store) 
            : base(link, schema, store)
        {
            
        }

        IEndpointSchema IConsumer<TEvent>.Schema => Schema;


        IEndpointSchema IPublisher<TEvent>.Schema => Schema;
        


        public bool ExchangePassive() => GetParameter(nameof(ExchangePassive), false);
        public IEventEndpoint<TService, TEvent> ExchangePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(ExchangePassive), value));
        
        public string NamedProducer() => GetParameter(nameof(NamedProducer), (string) null);
        public IEventEndpoint<TService, TEvent> NamedProducer(string value) 
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(NamedProducer), value));

        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IEventEndpoint<TService, TEvent> ConfirmsMode(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(ConfirmsMode), value));

        public string QueueName() => GetParameter(nameof(QueueName),
            $"{Link.HolderName}.{Schema.Service.Owner}.{Schema.Service.Name}.{Schema.Name}");
        public IEventEndpoint<TService, TEvent> QueueName(string value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(QueueName), value));

        public ushort PrefetchCount() => GetParameter<ushort>(nameof(PrefetchCount), 1);
        public IEventEndpoint<TService, TEvent> PrefetchCount(ushort value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(PrefetchCount), value));


        public bool AutoAck() => GetParameter(nameof(AutoAck), false);
        public IEventEndpoint<TService, TEvent> AutoAck(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetParameter(nameof(ErrorStrategy), (ILinkConsumerErrorStrategy) null);
        public IEventEndpoint<TService, TEvent> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetParameter(nameof(CancelOnHaFailover), (bool?) null);
        public IEventEndpoint<TService, TEvent> CancelOnHaFailover(bool? value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        public IEventEndpoint<TService, TEvent> Exclusive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(Exclusive), value));

        public bool QueuePassive() => GetParameter(nameof(QueuePassive), false);
        public IEventEndpoint<TService, TEvent> QueuePassive(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(QueuePassive), value));
        
        public bool Bind() => GetParameter(nameof(Bind), true);
        public IEventEndpoint<TService, TEvent> Bind(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetParameter(nameof(QueueParameters), new QueueParameters());
        public IEventEndpoint<TService, TEvent> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(QueueParameters), setter(QueueParameters())));


        public IEnumerable<string> RoutingKeys() => GetParameter(nameof(RoutingKeys), Enumerable.Empty<string>());
        public IEventEndpoint<TService, TEvent> AddRoutingKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var lst = RoutingKeys().ToList();
            lst.Add(value);
            return new EventEndpoint<TService, TEvent>(Link,
                Schema, SetParameter<IEnumerable<string>>(nameof(RoutingKeys), new ReadOnlyCollection<string>(lst)));
        }

        public IEventEndpoint<TService, TEvent> MessageTtl(TimeSpan? value)
           => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(MessageTtl), value));

        public TimeSpan? MessageTtl() => GetParameter(nameof(MessageTtl), (TimeSpan?) null);
        

        public IEventEndpoint<TService, TEvent> Persistent(bool value)
            => new EventEndpoint<TService, TEvent>(Link, Schema, SetParameter(nameof(Persisent), value));

        public bool Persisent()
            => TryGetParameter<bool>(nameof(Persisent)).IfNone(() => Schema.Exchange().Durable);

        /*
        public IEventEndpoint<TService, TEvent> AddRoutingKeyByExample(TEvent value)
        {
            if(Description.RoutingKeyExtractor == null)
                throw new InvalidOperationException($"Routing key by example not supported!");
            return AddRoutingKey(Description.RoutingKeyExtractor(value));
        }
        */
        public IDisposable Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener)
        {
            Log.Trace($"{nameof(Listen)} enter");
            try
            {
                var routingKeys = new List<string>();
                if (Schema.RoutingKey() != null)
                    routingKeys.Add(Schema.RoutingKey());
                
                else
                {
                    var t = RoutingKeys();
                    if (t != null)
                        routingKeys.AddRange(t);
                }

                var consumerBuilder = Utils.CreateConsumerBuilder(Link, Schema.Exchange(),
                    ExchangePassive(), QueuePassive(), QueueName(), AutoAck(), CancelOnHaFailover(), ErrorStrategy(),
                    Exclusive(), PrefetchCount(), QueueParameters(), routingKeys, Bind());


                var consumer = consumerBuilder.Handler(async msg =>
                {
                    var log = Log.With("@msg", msg);
                    log.Trace("Message receiving");
                    try
                    {
                        var body = (TEvent) Link.PayloadManager.Deserialize(msg, typeof(TEvent));
                        var ack = await listener(body, msg.Cancellation);
                        log.With("ack", ack).Trace("Message received");
                        switch (ack)
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
                        
                    }
                    catch (Exception ex)
                    {
                        log.Error("Message receiving error", ex);
                        throw;
                    }
                }).Build();
                Log.Trace($"{nameof(Listen)} success");
                return consumer;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Listen)} error", ex);
                throw;
            }
        }

        public async Task PublishAsync(TEvent message, CancellationToken token = default(CancellationToken))
        {
            var log = Log.With("@message", message);
            log.Trace($"{nameof(PublishAsync)} enter");
            try
            {
                
                var props = new LinkMessageProperties();
                var serialized = Link.PayloadManager.Serialize(ContentType, message, props);

                var msg = new LinkPublishMessage<byte[]>(serialized, props, new LinkPublishProperties
                {
                    RoutingKey =
                        Schema.Exchange().Type == ExchangeKind.Fanout
                            ? null
                            : Schema.RoutingKey() // ?? Schema.RoutingKeyExtractor(message)
                });
                var publisher = Utils.CreateProducer(Link, Schema.Exchange(), Schema.ContentType(),
                    ExchangePassive(),
                    ConfirmsMode(), NamedProducer());
                await publisher.PublishAsync(msg, token);
                log.Trace($"{nameof(PublishAsync)} success");
            }
            catch (Exception ex)
            {
                
                if(ex.IsCancellation())
                    log.Info($"{nameof(PublishAsync)} cancelled");
                else
                    log.Error($"{nameof(PublishAsync)} error", ex);
                throw;
            }
        }
    }
}