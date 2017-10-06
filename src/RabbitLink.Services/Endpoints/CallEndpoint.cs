using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using RabbitLink.Consumer;
using RabbitLink.Messaging;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Internals;

namespace RabbitLink.Services
{
    internal class CallEndpoint<TService, TArg, TResult> :  BuilderBase, ICallEndpoint<TService, TArg, TResult> 
    {
        private CallDescription Description { get; }
        private ServiceLink Link { get; }

        public CallEndpoint(ServiceLink link, CallDescription description)
        {
            Description = description;
            Link = link;
        }

        private CallEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }

        public ushort PrefetchCount() => GetValue(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg, TResult> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg, TResult>(Link, Description, SetValue(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetValue(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg, TResult> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Description, SetValue(nameof(Timeout), value));
        }
        
        public TimeSpan? Expires() => GetValue(nameof(Expires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg, TResult> Expires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Description, SetValue(nameof(Expires), value));
        }
        
        public bool Durable() => GetValue(nameof(Durable), false);
        public ICallEndpoint<TService, TArg, TResult> Durable(bool value)
            => new CallEndpoint<TService, TArg, TResult>(Link, Description, SetValue(nameof(Durable), value));

        public bool AutoDelete() => GetValue(nameof(AutoDelete), false);
        public ICallEndpoint<TService, TArg, TResult> AutoDelete(bool value)
            => new CallEndpoint<TService, TArg, TResult>(Link, Description, SetValue(nameof(AutoDelete), value));
        
        public IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor)
        {
            var consumerBuilder =
                Utils.CreateConsumerBuilder(Link, Description.RequestExchange,
                    false, false, Description.RpcQueueName, false, null, null, false,
                    PrefetchCount(), new QueueParameters().Durable(Durable()).AutoDelete(AutoDelete()),
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


        public async Task<TResult> Call(TArg arg, CancellationToken token)
        {
            CancellationTokenSource source = null;
            try
            {
                if (!token.CanBeCanceled)
                {
                    source = new CancellationTokenSource(Timeout());
                    token = source.Token;
                }
                var queueName = $"{Description.Service.Owner}.{Description.ResponseExchange}.{Guid.NewGuid():D}";
                var consumer = Link.GetOrAddConsumer(Description.ResponseExchange.Name ?? "",
                    () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Description.ResponseExchange,
                        true, false, queueName, false, null, null, false, PrefetchCount(),
                        new QueueParameters().Expires(Expires()),
                        new[] {queueName}, true), queueName));
                await consumer.WaitReadyAsync(token);
                var props = new LinkMessageProperties
                {
                    CorrelationId = Guid.NewGuid().ToString("D"),
                    ReplyTo = queueName
                };
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
    }

    internal class CallEndpoint<TService, TArg> : BuilderBase, ICallEndpoint<TService, TArg>
    {
        private CallDescription Description { get; }
        private ServiceLink Link { get; }
        
        public CallEndpoint(ServiceLink link, CallDescription description)
        {
            Description = description;
            Link = link;
        }

        private CallEndpoint(ServiceLink link, CallDescription description, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }

        public ushort PrefetchCount() => GetValue(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg>(Link, Description, SetValue(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetValue(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Description, SetValue(nameof(Timeout), value));
        }
        
        public TimeSpan? Expires() => GetValue(nameof(Expires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg> Expires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Description, SetValue(nameof(Expires), value));
        }
        
        public bool Durable() => GetValue(nameof(Durable), false);
        public ICallEndpoint<TService, TArg> Durable(bool value)
            => new CallEndpoint<TService, TArg>(Link, Description, SetValue(nameof(Durable), value));

        public bool AutoDelete() => GetValue(nameof(AutoDelete), false);
        public ICallEndpoint<TService, TArg> AutoDelete(bool value)
            => new CallEndpoint<TService, TArg>(Link, Description, SetValue(nameof(AutoDelete), value));

        public IDisposable Process(Func<TArg, CancellationToken, Task> processor)
        {
            var consumerBuilder =
                Utils.CreateConsumerBuilder(Link, Description.RequestExchange,
                    false, false, Description.RpcQueueName, false, null, null, false,
                    PrefetchCount(), new QueueParameters().Durable(Durable()).AutoDelete(AutoDelete()),
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
        
        public async Task Call(TArg arg, CancellationToken token)
        {
            CancellationTokenSource source = null;
            try
            {
                if (!token.CanBeCanceled)
                {
                    source = new CancellationTokenSource(Timeout());
                    token = source.Token;
                }
                var queueName = $"{Description.Service.Owner}.{Description.ResponseExchange}.{Guid.NewGuid():D}";
                var consumer = Link.GetOrAddConsumer(Description.ResponseExchange.Name ?? "",
                    () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Description.ResponseExchange,
                        true, false, queueName, false, null, null, false, PrefetchCount(),
                        new QueueParameters().Expires(Expires()),
                        new[] {queueName}, true), queueName));
                await consumer.WaitReadyAsync(token);
                var props = new LinkMessageProperties
                {
                    CorrelationId = Guid.NewGuid().ToString("D"),
                    ReplyTo = queueName
                };
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
    }
}