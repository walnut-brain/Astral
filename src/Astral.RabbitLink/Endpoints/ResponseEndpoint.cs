using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;
using Astral.Schema.RabbitMq;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    internal class ResponseEndpoint<TService, TRequest, TResponse> : Endpoint<IRabbitMqCallSchema>, 
        IResponseEndpoint<TService, TRequest, TResponse>
    {
        
        public ResponseEndpoint(ServiceLink link, IRabbitMqCallSchema schema)
            : base(link, schema)
        {
            
        }

        private ResponseEndpoint(ServiceLink link, IRabbitMqCallSchema schema, IReadOnlyDictionary<string, object> store) 
            : base(link, schema, store)
        {
        }

        IEndpointSchema IPublisher<Response<TResponse>>.Schema => Schema;


        IEndpointSchema IConsumer<Request<TRequest>>.Schema => Schema;
        

        public ushort PrefetchCount() => GetParameter(nameof(PrefetchCount), (ushort) 1);
        public IResponseEndpoint<TService, TRequest, TResponse> PrefetchCount(ushort value)
            => new ResponseEndpoint<TService, TRequest, TResponse>(Link, Schema, SetParameter(nameof(PrefetchCount), value));
        
        
        
        public bool ConfirmsMode() => GetParameter(nameof(ConfirmsMode), true);
        public IResponseEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value)
            => new ResponseEndpoint<TService, TResponse, TRequest>(Link, Schema, SetParameter(nameof(ConfirmsMode), value));

        public async Task PublishAsync(Response<TResponse> message, CancellationToken token = default(CancellationToken))
        {
            var log = Log.With("@message", message);
            log.Trace($"{nameof(PublishAsync)} enter");
            try
            {
                var publisher = Utils.CreateProducer(Link, Schema.ResponseExchange, Schema.ContentType,
                    false, ConfirmsMode());
                var props = new LinkMessageProperties
                {
                    CorrelationId = message.CorrelationId
                };
                var serializer = Link.PayloadManager;
                var answer = new LinkPublishMessage<byte[]>(message.IsFail
                        ? serializer.Serialize(Schema.ContentType, new RpcFail
                        {
                            Kind = message.Error.GetType().FullName,
                            Message = message.Error.Message
                        }, props)
                        : serializer.Serialize(Schema.ContentType, message.Result, props),
                    props,
                    new LinkPublishProperties
                    {
                        RoutingKey = message.ReplyTo
                    });
                await publisher.PublishAsync(answer, token);
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

        public IDisposable Listen(Func<Request<TRequest>, CancellationToken, Task<Acknowledge>> listener)
        {
            Log.Trace($"{nameof(Listen)} enter");
            try
            {
                var queue = Schema.RequestQueue;
                var consumerBuilder =
                    Utils.CreateConsumerBuilder(Link, Schema.Exchange,
                        false, false, queue.Name, false, null, null, false,
                        PrefetchCount(),
                        new QueueParameters().Durable(queue.Durable).AutoDelete(queue.AutoDelete),
                        new[] {Schema.RoutingKey}, true);

                consumerBuilder = consumerBuilder.Handler(async msg =>
                {
                    var log = Log.With("@msg", msg);
                    log.Trace("Message receiving");
                    try
                    {
                        var data = (TRequest) Link.PayloadManager.Deserialize(msg, typeof(TRequest));
                        var request = new Request<TRequest>(msg.Properties.CorrelationId, msg.Properties.ReplyTo, data);
                        var ack = await listener(request, msg.Cancellation);
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
                        ;
                    }
                    catch (Exception ex)
                    {
                        if(ex.IsCancellation())
                            log.Trace("Message receiving cancelled");
                        else
                            log.Error("Message receiving error", ex);
                        throw;
                    }
                });

                var consumer = consumerBuilder.Build();
                Log.Trace($"{nameof(Listen)} success");
                return consumer;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Listen)} error", ex);
                throw;
            }
        }
    }
}