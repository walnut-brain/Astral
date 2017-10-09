using System;
using System.Collections.Generic;
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
    internal class RequestEndpoint<TService, TRequest, TResponse> : Endpoint<CallDescription>
        , IRequestEndpoint<TService, TRequest, TResponse>
    {
        

        public RequestEndpoint(ServiceLink link, CallDescription description)
            : base(link, description)
        {
        }

        private RequestEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) 
            : base(link, description, store)
        {
            
        }

        public bool ExchangePassive() => GetParameter(nameof(ExchangePassive), false);
        public IRequestEndpoint<TService, TResponse, TRequest> ExchangePassive(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(ExchangePassive), value));
        
        public string NamedProducer() => GetParameter(nameof(NamedProducer), (string) null);
        public IRequestEndpoint<TService, TResponse, TRequest> NamedProducer(string value) 
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(NamedProducer), value));

        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IRequestEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(ConfirmsMode), value));

        public string QueueName() => GetParameter(nameof(QueueName),
            $"{Link.HolderName}.{Description.Service.Owner}.{Description.Service.Name}.{Description.Name}");
        public IRequestEndpoint<TService, TResponse, TRequest> QueueName(string value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(QueueName), value));

        public ushort PrefetchCount() => GetParameter<ushort>(nameof(PrefetchCount), 1);
        public IRequestEndpoint<TService, TResponse, TRequest> PrefetchCount(ushort value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(PrefetchCount), value));


        public bool AutoAck() => GetParameter(nameof(AutoAck), false);
        public IRequestEndpoint<TService, TResponse, TRequest> AutoAck(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(AutoAck), value));

        public ILinkConsumerErrorStrategy ErrorStrategy() =>
            GetParameter(nameof(ErrorStrategy), (ILinkConsumerErrorStrategy) null);
        public IRequestEndpoint<TService, TResponse, TRequest> ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(ErrorStrategy), value));


        public bool? CancelOnHaFailover() => GetParameter(nameof(CancelOnHaFailover), (bool?)null);
        public IRequestEndpoint<TService, TResponse, TRequest> CancelOnHaFailover(bool? value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(CancelOnHaFailover), value));

        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        public IRequestEndpoint<TService, TResponse, TRequest> Exclusive(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(Exclusive), value));

        public bool QueuePassive() => GetParameter(nameof(QueuePassive), false);
        public IRequestEndpoint<TService, TResponse, TRequest> QueuePassive(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(QueuePassive), value));
        
        public bool Bind() => GetParameter(nameof(Bind), true);
        public IRequestEndpoint<TService, TResponse, TRequest> Bind(bool value)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(Bind), value));
        
        public QueueParameters QueueParameters() => GetParameter(nameof(QueueParameters), new QueueParameters());
        public IRequestEndpoint<TService, TResponse, TRequest> QueueParameters(Func<QueueParameters, QueueParameters> setter)
            => new RequestEndpoint<TService, TResponse, TRequest>(Link, Description, SetParameter(nameof(QueueParameters), setter(QueueParameters())));
        
        public IDisposable Listen(Func<Response<TResponse>, CancellationToken, Task<Acknowledge>> listener)
        {
            var routingKeys = new List<string>();
            routingKeys.Add(QueueName());
            

            var consumerBuilder = Utils.CreateConsumerBuilder(Link, Description.ResponseExchange,
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
                ReplyTo = QueueName()
            };
            var serialized = Link.PayloadManager.Serialize(ContentType, message, props);
            
            var msg = new LinkPublishMessage<byte[]>(serialized, props, new LinkPublishProperties
            {
                RoutingKey =
                    Description.RequestExchange.Type == LinkExchangeType.Fanout ? null :
                        Description.RoutingKey
                
            });
            var publisher = Utils.CreateProducer(Link, Description.RequestExchange, Description.ContentType, ExchangePassive(),
                ConfirmsMode(), NamedProducer());
            return publisher.PublishAsync(msg, token);
        }
    }
}