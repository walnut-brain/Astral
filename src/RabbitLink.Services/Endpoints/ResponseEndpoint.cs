﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Links;
using RabbitLink.Consumer;
using RabbitLink.Messaging;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Internals;

namespace RabbitLink.Services
{
    internal class ResponseEndpoint<TService, TRequest, TResponse> : BuilderBase, IResponseEndpoint<TService, TRequest, TResponse>
    {
        private CallDescription Description { get; }
        private ServiceLink Link { get; }

        public ResponseEndpoint(ServiceLink link, CallDescription description)
        {
            Description = description;
            Link = link;
        }

        private ResponseEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }
        
        public ushort PrefetchCount() => GetValue(nameof(PrefetchCount), (ushort) 1);
        public IResponseEndpoint<TService, TRequest, TResponse> PrefetchCount(ushort value)
            => new ResponseEndpoint<TService, TRequest, TResponse>(Link, Description, SetValue(nameof(PrefetchCount), value));
        
        
        public bool Durable() => GetValue(nameof(Durable), false);
        public IResponseEndpoint<TService, TRequest, TResponse> Durable(bool value)
            => new ResponseEndpoint<TService, TRequest, TResponse>(Link, Description, SetValue(nameof(Durable), value));

        public bool AutoDelete() => GetValue(nameof(AutoDelete), false);
        public IResponseEndpoint<TService, TRequest, TResponse> AutoDelete(bool value)
            => new ResponseEndpoint<TService, TRequest, TResponse>(Link, Description, SetValue(nameof(AutoDelete), value));
        
        public bool ConfirmsMode() => GetValue(nameof(ConfirmsMode), true);
        public IResponseEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value)
            => new ResponseEndpoint<TService, TResponse, TRequest>(Link, Description, SetValue(nameof(ConfirmsMode), value));

        public async Task PublishAsync(Response<TResponse> message, CancellationToken token = default(CancellationToken))
        {
            var publisher = Utils.CreateProducer(Link, Description.ResponseExchange, Description.ContentType,
                false, ConfirmsMode());
            var props = new LinkMessageProperties
            {
                CorrelationId = message.CorrelationId
            };
            var serializer = Link.PayloadManager;
            var answer = new LinkPublishMessage<byte[]>(message.IsFail ? serializer.Serialize(Description.ContentType, new RpcFail {
                Kind    = message.Error.GetType().FullName,
                Message = message.Error.Message }, props) : serializer.Serialize(Description.ContentType, message.Result, props),  
                props,
                new LinkPublishProperties
                {
                    RoutingKey = message.ReplyTo
                });
            await publisher.PublishAsync(answer, token);
        }

        public IDisposable Listen(Func<Request<TRequest>, CancellationToken, Task<Acknowledge>> listener)
        {
            var consumerBuilder =
                Utils.CreateConsumerBuilder(Link, Description.RequestExchange,
                    false, false, Description.RpcQueueName, false, null, null, false,
                    PrefetchCount(), new QueueParameters().Durable(Durable()).AutoDelete(AutoDelete()),
                    new[] {Description.RoutingKey}, true);
            
            consumerBuilder.Handler(async msg =>
            {
                var data = (TRequest) Link.PayloadManager.Deserialize(msg, typeof(TRequest));
                var request = new Request<TRequest>(msg.Properties.CorrelationId, msg.Properties.ReplyTo, data);
                switch (await listener(request, msg.Cancellation))
                {
                    case Acknowledge.Ack:
                        return LinkConsumerAckStrategy.Ack;
                    case Acknowledge.Nack:
                        return LinkConsumerAckStrategy.Nack;
                    case Acknowledge.Requeue:
                        return LinkConsumerAckStrategy.Requeue;
                    default:
                        throw new ArgumentOutOfRangeException();
                };
            });

            return consumerBuilder.Build();
        }
    }
}