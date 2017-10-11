using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.Markup.RabbitMq;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;
using RabbitLink.Consumer;
using RabbitLink.Messaging;
using RabbitLink.Topology;

namespace Astral.RabbitLink
{
    internal class RequestEndpoint<TService, TRequest, TResponse> : Endpoint<ICallSchema>
        , IRequestEndpoint<TService, TRequest, TResponse>
    {
        

        public RequestEndpoint(ServiceLink link, ICallSchema schema)
            : base(link, schema)
        {
        }

        private RequestEndpoint(ServiceLink link, ICallSchema schema, IReadOnlyDictionary<string, object> store) 
            : base(link, schema, store)
        {
            
        }

        IEndpointSchema IConsumer<Response<TResponse>>.Schema => Schema;


        IEndpointSchema IPublisher<Request<TRequest>>.Schema => Schema;
        

        public bool ExchangePassive() => GetParameter(nameof(ExchangePassive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> ExchangePassive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(ExchangePassive), value));
        
        public string NamedProducer() => GetParameter(nameof(NamedProducer), (string) null);
        public IRequestEndpoint<TService, TRequest, TResponse> NamedProducer(string value) 
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(NamedProducer), value));

        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IRequestEndpoint<TService, TRequest, TResponse> ConfirmsMode(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(ConfirmsMode), value));

        public string QueueName() => GetParameter(nameof(QueueName),
            $"{Link.HolderName}.{Schema.Service.Owner}.{Schema.Service.Name}.{Schema.Name}");
        public IRequestEndpoint<TService, TRequest, TResponse> QueueName(string value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(QueueName), value));

        public ushort PrefetchCount() => GetParameter<ushort>(nameof(PrefetchCount), 1);
        public IRequestEndpoint<TService, TRequest, TResponse> PrefetchCount(ushort value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(PrefetchCount), value));


        public bool AutoAck() => GetParameter(nameof(AutoAck), false);
        public IRequestEndpoint<TService, TRequest, TResponse> AutoAck(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetParameter(nameof(ErrorStrategy), (ILinkConsumerErrorStrategy) null);
        public IRequestEndpoint<TService, TRequest, TResponse> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetParameter(nameof(CancelOnHaFailover), (bool?)null);
        public IRequestEndpoint<TService, TRequest, TResponse> CancelOnHaFailover(bool? value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> Exclusive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(Exclusive), value));

        public bool QueuePassive() => GetParameter(nameof(QueuePassive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> QueuePassive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(QueuePassive), value));
        
        public bool Bind() => GetParameter(nameof(Bind), true);
        public IRequestEndpoint<TService, TRequest, TResponse> Bind(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetParameter(nameof(QueueParameters), new QueueParameters());
        public IRequestEndpoint<TService, TRequest, TResponse> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(QueueParameters), setter(QueueParameters())));

        public IRequestEndpoint<TService, TRequest, TResponse> MessageTtl(TimeSpan? value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(MessageTtl), value));

        public TimeSpan? MessageTtl()
            => GetParameter(nameof(MessageTtl), (TimeSpan?) null);

        public IRequestEndpoint<TService, TRequest, TResponse> Persistent(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(Persisent), value));

        public bool Persisent()
            => TryGetParameter<bool>(nameof(Persisent)).IfNone(() => Schema.Exchange().Durable);

        public IDisposable Listen(Func<Response<TResponse>, CancellationToken, Task<Acknowledge>> listener)
        {
            Log.Trace($"{nameof(Listen)} enter");
            try
            {
                var routingKeys = new List<string>();
                routingKeys.Add(QueueName());


                var consumerBuilder = Utils.CreateConsumerBuilder(Link, Schema.ResponseExchange(),
                    ExchangePassive(), QueuePassive(), QueueName(), AutoAck(), CancelOnHaFailover(), ErrorStrategy(),
                    Exclusive(), PrefetchCount(), QueueParameters(), routingKeys, Bind());


                var consumer = consumerBuilder.Handler(async msg =>
                {
                    var log = Log.With("@msg", msg);
                    log.Trace("Message recieving");
                    try
                    {
                        var obj = Link.PayloadManager.Deserialize(msg, typeof(TResponse));
                        Response<TResponse> response;
                        switch (obj)
                        {
                            case TResponse resp:
                                response = new Response<TResponse>(resp, msg.Properties.CorrelationId, null);
                                break;

                            case Exception ex:
                                response = new Response<TResponse>(ex, msg.Properties.CorrelationId, null);
                                break;
                            case RpcFail fail:
                                response = new Response<TResponse>(new RpcFailException(fail.Message, fail.Kind),
                                    msg.Properties.CorrelationId, null);
                                break;
                            default:
                                return LinkConsumerAckStrategy.Nack;
                        }

                        var result = await listener(response, msg.Cancellation);
                        log.With("ack", result).Trace("Message recieved");
                        switch (result)
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
                        if(ex.IsCancellation())
                            log.Trace("Cancelled");
                        else
                            log.Error("Message recieving error", ex);
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

        public async Task PublishAsync(Request<TRequest> message, CancellationToken token = default(CancellationToken))
        {
            var log = Log.With("@message", message);
            log.Trace($"{nameof(PublishAsync)} enter");
            try
            {
                var props = new LinkMessageProperties
                {
                    CorrelationId = message.CorrelationId,
                    ReplyTo = QueueName()
                };
                var serialized = Link.PayloadManager.Serialize(ContentType, message, props);

                var msg = new LinkPublishMessage<byte[]>(serialized, props, new LinkPublishProperties
                {
                    RoutingKey =
                        Schema.Exchange().Type == ExchangeKind.Fanout ? null : Schema.RoutingKey()

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