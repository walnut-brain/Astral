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
    internal class RequestEndpoint<TService, TRequest, TResponse> : Endpoint<CallSchema>
        , IRequestEndpoint<TService, TRequest, TResponse>
    {
        

        public RequestEndpoint(ServiceLink link, CallSchema description)
            : base(link, description)
        {
        }

        private RequestEndpoint(ServiceLink link, CallSchema description, IReadOnlyDictionary<string, object> store) 
            : base(link, description, store)
        {
            
        }

        public bool ExchangePassive() => GetParameter(nameof(ExchangePassive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> ExchangePassive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(ExchangePassive), value));
        
        public string NamedProducer() => GetParameter(nameof(NamedProducer), (string) null);
        public IRequestEndpoint<TService, TRequest, TResponse> NamedProducer(string value) 
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(NamedProducer), value));

        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IRequestEndpoint<TService, TRequest, TResponse> ConfirmsMode(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(ConfirmsMode), value));

        public string QueueName() => GetParameter(nameof(QueueName),
            $"{Link.HolderName}.{Description.Service.Owner}.{Description.Service.Name}.{Description.Name}");
        public IRequestEndpoint<TService, TRequest, TResponse> QueueName(string value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(QueueName), value));

        public ushort PrefetchCount() => GetParameter<ushort>(nameof(PrefetchCount), 1);
        public IRequestEndpoint<TService, TRequest, TResponse> PrefetchCount(ushort value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(PrefetchCount), value));


        public bool AutoAck() => GetParameter(nameof(AutoAck), false);
        public IRequestEndpoint<TService, TRequest, TResponse> AutoAck(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetParameter(nameof(ErrorStrategy), (ILinkConsumerErrorStrategy) null);
        public IRequestEndpoint<TService, TRequest, TResponse> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetParameter(nameof(CancelOnHaFailover), (bool?)null);
        public IRequestEndpoint<TService, TRequest, TResponse> CancelOnHaFailover(bool? value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> Exclusive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(Exclusive), value));

        public bool QueuePassive() => GetParameter(nameof(QueuePassive), false);
        public IRequestEndpoint<TService, TRequest, TResponse> QueuePassive(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(QueuePassive), value));
        
        public bool Bind() => GetParameter(nameof(Bind), true);
        public IRequestEndpoint<TService, TRequest, TResponse> Bind(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetParameter(nameof(QueueParameters), new QueueParameters());
        public IRequestEndpoint<TService, TRequest, TResponse> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(QueueParameters), setter(QueueParameters())));

        public IRequestEndpoint<TService, TRequest, TResponse> MessageTtl(TimeSpan? value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(MessageTtl), value));

        public TimeSpan? MessageTtl()
            => GetParameter(nameof(MessageTtl), (TimeSpan?) null);

        public IRequestEndpoint<TService, TRequest, TResponse> Persistent(bool value)
            => new RequestEndpoint<TService, TRequest, TResponse>(Link, Description, SetParameter(nameof(Persisent), value));

        public bool Persisent()
            => TryGetParameter<bool>(nameof(Persisent)).IfNone(() => Description.Exchange().Durable);

        public IDisposable Listen(Func<Response<TResponse>, CancellationToken, Task<Acknowledge>> listener)
        {
            var routingKeys = new List<string>();
            routingKeys.Add(QueueName());
            

            var consumerBuilder = Utils.CreateConsumerBuilder(Link, Description.ResponseExchange(),
                ExchangePassive(), QueuePassive(), QueueName(), AutoAck(), CancelOnHaFailover(), ErrorStrategy(),
                Exclusive(), PrefetchCount(), QueueParameters(), routingKeys, Bind());
            
            
            return consumerBuilder.Handler(async msg =>
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

                switch (await listener(response, msg.Cancellation))
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

        public Task PublishAsync(Request<TRequest> message, CancellationToken token = default(CancellationToken))
        {
            var props = new LinkMessageProperties
            {
                CorrelationId = message.CorrelationId,
                ReplyTo = QueueName(),
                Expiration = MessageTtl(),
                DeliveryMode = Persisent() ? LinkDeliveryMode.Persistent : LinkDeliveryMode.Transient
            };
            var serialized = Link.PayloadManager.Serialize(ContentType, message, props);
            
            var msg = new LinkPublishMessage<byte[]>(serialized, props, new LinkPublishProperties
            {
                RoutingKey =
                    Description.Exchange().Type == ExchangeKind.Fanout ? null :
                        Description.RoutingKey()
                
            });
            var publisher = Utils.CreateProducer(Link, Description.Exchange(), Description.ContentType(), ExchangePassive(),
                ConfirmsMode(), NamedProducer());
            return publisher.PublishAsync(msg, token);
        }
    }
}