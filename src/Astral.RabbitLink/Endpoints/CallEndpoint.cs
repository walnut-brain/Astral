using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    internal class CallEndpoint<TService, TArg, TResult> :  Endpoint<CallDescription>, ICallEndpoint<TService, TArg, TResult> 
    {
        

        public CallEndpoint(ServiceLink link, CallDescription description)
            : base(link, description)
        {
        }

        private CallEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) 
            : base(link, description, store)
        {
        }

        public ushort PrefetchCount() => GetParameter(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg, TResult> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg, TResult>(Link, Description, SetParameter(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetParameter(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg, TResult> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Description, SetParameter(nameof(Timeout), value));
        }
        
        public TimeSpan? ResponseQueueExpires() => GetParameter(nameof(ResponseQueueExpires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg, TResult> ResponseQueueExpires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Description, SetParameter(nameof(ResponseQueueExpires), value));
        }
        
        public IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor)
        {
            var consumerBuilder =
                Utils.CreateConsumerBuilder(Link, Description.RequestExchange,
                    false, false, Description.QueueName, false, null, null, false,
                    PrefetchCount(), 
                    new QueueParameters().Durable(Description.QueueDurable).AutoDelete(Description.QueueAutoDelete),
                    new[] {Description.RoutingKey}, true);
            
            var publisher = Utils.CreateProducer(Link, Description.ResponseExchange, Description.ContentType,
                false, false);
            consumerBuilder = consumerBuilder.Handler(async msg =>
            {
                var data = (TArg) Link.PayloadManager.Deserialize(msg, typeof(TArg));
                var tsk = processor(data, msg.Cancellation);
                try
                {
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = msg.Properties.CorrelationId
                    };
                    var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, await tsk, props),
                        props,
                        new LinkPublishProperties
                        {
                            RoutingKey = msg.Properties.ReplyTo
                        });
                    await publisher.PublishAsync(answer, msg.Cancellation);
                }
                catch(Exception ex)
                {
                    if (tsk.IsCanceled)
                        return LinkConsumerAckStrategy.Requeue;
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = msg.Properties.CorrelationId
                    };
                    var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, new RpcFail
                        {
                            Kind    = ex.GetType().FullName,
                            Message = ex.Message
                        }, props),
                        props,
                        new LinkPublishProperties
                        {
                            RoutingKey = msg.Properties.ReplyTo
                        });
                    await publisher.PublishAsync(answer, msg.Cancellation);
                }
                return LinkConsumerAckStrategy.Ack;
            });

            return consumerBuilder.Build();
        }


        private async Task<TResult> Call(TArg arg, CancellationToken token, TimeSpan timeout)
        {
            CancellationTokenSource source = null;
            TimeSpan? messageTtl = null;
            try
            {
                if (!token.CanBeCanceled)
                {
                    source = new CancellationTokenSource(timeout);
                    token = source.Token;
                    messageTtl = timeout;
                }
                var queueName = $"{Description.Service.Owner}.{Description.ResponseExchange}.{Guid.NewGuid():D}";
                var consumer = Link.GetOrAddConsumer(Description.ResponseExchange.Name ?? "",
                    () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Description.ResponseExchange,
                        true, false, queueName, false, null, null, false, PrefetchCount(),
                        new QueueParameters().Expires(ResponseQueueExpires()),
                        new[] {queueName}, true), queueName));
                //await consumer.WaitReadyAsync(token);
                var props = new LinkMessageProperties
                {
                    CorrelationId = Guid.NewGuid().ToString("D"),
                    ReplyTo = queueName
                };
                if (messageTtl != null) 
                    props.Expiration = messageTtl.Value;
                var waiter = consumer.WaitFor<TResult>(props.CorrelationId, token);
                var producer = Utils.CreateProducer(Link, Description.RequestExchange, Description.ContentType, true);
                
                var request = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, arg, props),
                    props, new LinkPublishProperties
                    {
                        RoutingKey = Description.RoutingKey
                    });
                await producer.PublishAsync(request, token);
                return await waiter;

            }
            finally
            {
                source?.Dispose();
            }
        }

        public Task<TResult> Call(TArg arg, CancellationToken token = default(CancellationToken))
            => Call(arg, token, Timeout());

        public Task<TResult> Call(TArg arg, TimeSpan timeout)
            => Call(arg, CancellationToken.None, timeout);
    }

    internal class CallEndpoint<TService, TArg> : Endpoint<CallDescription>, ICallEndpoint<TService, TArg>
    {
        public CallEndpoint(ServiceLink link, CallDescription description)
            : base(link, description)
        {
        }

        private CallEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) 
            : base(link, description, store)
        {
        }

        public ushort PrefetchCount() => GetParameter(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg>(Link, Description, SetParameter(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetParameter(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Description, SetParameter(nameof(Timeout), value));
        }
        
        public TimeSpan? ResponseQueueExpires() => GetParameter(nameof(ResponseQueueExpires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg> ResponseQueueExpires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Description, SetParameter(nameof(ResponseQueueExpires), value));
        }
        

        public IDisposable Process(Func<TArg, CancellationToken, Task> processor)
        {
            var consumerBuilder =
                Utils.CreateConsumerBuilder(Link, Description.RequestExchange,
                    false, false, Description.QueueName, false, null, null, false,
                    PrefetchCount(), new QueueParameters().Durable(Description.QueueDurable).AutoDelete(Description.QueueAutoDelete),
                    new[] {Description.RoutingKey}, true);
            
            var publisher = Utils.CreateProducer(Link, Description.ResponseExchange, Description.ContentType,
                false, false);
            consumerBuilder.Handler(async msg =>
            {
                var data = (TArg) Link.PayloadManager.Deserialize(msg, typeof(TArg));
                var tsk = processor(data, msg.Cancellation);
                try
                {
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = msg.Properties.CorrelationId
                    };
                    await tsk;
                    var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, new RpcOk(), props),
                        props,
                        new LinkPublishProperties
                        {
                            RoutingKey = msg.Properties.ReplyTo
                        });
                    await publisher.PublishAsync(answer, msg.Cancellation);
                }
                catch(Exception ex)
                {
                    if (tsk.IsCanceled)
                        return LinkConsumerAckStrategy.Requeue;
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = msg.Properties.CorrelationId
                    };
                    var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, new RpcFail
                        {
                            Kind    = ex.GetType().FullName,
                            Message = ex.Message
                        }, props),
                        props,
                        new LinkPublishProperties
                        {
                            RoutingKey = msg.Properties.ReplyTo
                        });
                    await publisher.PublishAsync(answer, msg.Cancellation);
                }
                return LinkConsumerAckStrategy.Ack;
            });

            return consumerBuilder.Build();
        }
        
        private async Task Call(TArg arg, CancellationToken token, TimeSpan timeout)
        {
            CancellationTokenSource source = null;
            TimeSpan? messageTtl = null;
            try
            {
                if (!token.CanBeCanceled)
                {
                    source = new CancellationTokenSource(timeout);
                    token = source.Token;
                    messageTtl = timeout;
                }
                var queueName = $"{Description.Service.Owner}.{Description.ResponseExchange}.{Guid.NewGuid():D}";
                var consumer = Link.GetOrAddConsumer(Description.ResponseExchange.Name ?? "",
                    () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Description.ResponseExchange,
                        true, false, queueName, false, null, null, false, PrefetchCount(),
                        new QueueParameters().Expires(ResponseQueueExpires()),
                        new[] {queueName}, true), queueName));
                //await consumer.WaitReadyAsync(token);
                var props = new LinkMessageProperties
                {
                    CorrelationId = Guid.NewGuid().ToString("D"),
                    ReplyTo = queueName
                };
                if (messageTtl != null)
                    props.Expiration = messageTtl.Value;
                var waiter = consumer.WaitFor<RpcOk>(props.CorrelationId, token);
                var producer = Utils.CreateProducer(Link, Description.RequestExchange, Description.ContentType, true);
                
                var request = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(Description.ContentType, arg, props),
                    props, new LinkPublishProperties
                    {
                        RoutingKey = Description.RoutingKey
                    });
                await producer.PublishAsync(request, token);
                await waiter;

            }
            finally
            {
                source?.Dispose();
            }
        }

        public Task Call(TArg arg, CancellationToken token)
            => Call(arg, token, Timeout());

        public Task Call(TArg arg, TimeSpan timeout)
            => Call(arg, CancellationToken.None, timeout);
    }
}