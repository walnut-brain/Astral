using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    internal class CallEndpoint<TService, TArg, TResult> :  Endpoint<ICallSchema>, ICallEndpoint<TService, TArg, TResult> 
    {

        public CallEndpoint(ServiceLink link, ICallSchema schema)
            : base(link, schema)
        {
        }

        private CallEndpoint(ServiceLink link, ICallSchema schema, IReadOnlyDictionary<string, object> store) 
            : base(link, schema, store)
        {
        }

        ICallSchema ICallClient<TArg, TResult>.Schema => Schema;

        ICallSchema ICallServer<TArg, TResult>.Schema => Schema;
        


        public ushort PrefetchCount() => GetParameter(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg, TResult> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg, TResult>(Link, Schema, SetParameter(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetParameter(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg, TResult> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Schema, SetParameter(nameof(Timeout), value));
        }
        
        public TimeSpan? ResponseQueueExpires() => GetParameter(nameof(ResponseQueueExpires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg, TResult> ResponseQueueExpires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg, TResult>(Link, Schema, SetParameter(nameof(ResponseQueueExpires), value));
        }
        
        public IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor)
        {
            Log.Trace($"{nameof(Process)} enter");
            try
            {
                var queue = Schema.RequestQueue();
                var consumerBuilder =
                    Utils.CreateConsumerBuilder(Link, Schema.Exchange(),
                        false, false, queue.Name, false, null, null, false,
                        PrefetchCount(),
                        new QueueParameters().Durable(queue.Durable).AutoDelete(queue.AutoDelete),
                        new[] {Schema.RoutingKey()}, true);

                var publisher = Utils.CreateProducer(Link, Schema.ResponseExchange(), Schema.ContentType(),
                    false, false);
                consumerBuilder = consumerBuilder.Handler(async msg =>
                {
                    var log = Log.With("@msg", msg);
                    log.Trace("Call receiving");
                    try
                    {
                        var data = (TArg) Link.PayloadManager.Deserialize(msg, typeof(TArg));
                        var tsk = processor(data, msg.Cancellation);
                        try
                        {
                            var props = new LinkMessageProperties
                            {
                                CorrelationId = msg.Properties.CorrelationId
                            };
                            var result = await tsk;
                            var answer = new LinkPublishMessage<byte[]>(
                                Link.PayloadManager.Serialize(Schema.ContentType(), result, props),
                                props,
                                new LinkPublishProperties
                                {
                                    RoutingKey = msg.Properties.ReplyTo
                                });
                            await publisher.PublishAsync(answer, msg.Cancellation);
                            log.With("@result", result).Trace("Call executed");
                        }
                        catch (Exception ex)
                        {

                            if (tsk.IsCanceled)
                                return LinkConsumerAckStrategy.Requeue;
                            var props = new LinkMessageProperties
                            {
                                CorrelationId = msg.Properties.CorrelationId
                            };
                            var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(
                                    Schema.ContentType(), new RpcFail
                                    {
                                        Kind = ex.GetType().FullName,
                                        Message = ex.Message
                                    }, props),
                                props,
                                new LinkPublishProperties
                                {
                                    RoutingKey = msg.Properties.ReplyTo
                                });
                            await publisher.PublishAsync(answer, msg.Cancellation);
                            log.Trace("Call executed with exception", ex);
                        }
                        return LinkConsumerAckStrategy.Ack;
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsCancellation())
                            log.Info("Cancelled");
                        else
                            log.Error("Error receiving", ex);
                        throw;
                    }
                });

                var consumer = consumerBuilder.Build();
                Log.Trace($"{nameof(Process)} success");
                return consumer;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Process)} error", ex);
                throw;
            }

            
        }


        private async Task<TResult> Call(TArg arg, CancellationToken token, TimeSpan timeout)
        {
            var log = Log.With("@message", arg);
            log.Trace($"{nameof(Call)} enter");
            try
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
                    var queueName = $"{Schema.Service.Owner}.{Schema.ResponseExchange()}.{Guid.NewGuid():D}";
                    var consumer = Link.GetOrAddConsumer(Schema.ResponseExchange().Name ?? "",
                        () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Schema.ResponseExchange(),
                            true, false, queueName, false, null, null, false, PrefetchCount(),
                            new QueueParameters().Expires(ResponseQueueExpires()),
                            new[] {queueName}, true), queueName));
                    await consumer.WaitReadyAsync(token);
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = Guid.NewGuid().ToString("D"),
                        ReplyTo = queueName
                    };
                    if (messageTtl != null)
                        props.Expiration = messageTtl.Value;
                    var waiter = consumer.WaitFor<TResult>(props.CorrelationId, token);
                    var producer =
                        Utils.CreateProducer(Link, Schema.Exchange(), Schema.ContentType(), true);

                    var request = new LinkPublishMessage<byte[]>(
                        Link.PayloadManager.Serialize(Schema.ContentType(), arg, props),
                        props, new LinkPublishProperties
                        {
                            RoutingKey = Schema.RoutingKey()
                        });
                    await producer.PublishAsync(request, token);
                    var result = await waiter;
                    log.With("@result", result).Trace($"{nameof(Call)} executed");
                    return result;

                }
                finally
                {
                    source?.Dispose();
                }
            }
            catch (Exception ex)
            {
                if(ex.IsCancellation())
                    log.Info($"{nameof(Call)} cancelled");
                else
                    log.Error($"{nameof(Call)} error", ex);
                throw;
            }
        }

        public Task<TResult> Call(TArg arg, CancellationToken token = default(CancellationToken))
            => Call(arg, token, Timeout());

        public Task<TResult> Call(TArg arg, TimeSpan timeout)
            => Call(arg, CancellationToken.None, timeout);
    }

    internal class CallEndpoint<TService, TArg> : Endpoint<ICallSchema>, ICallEndpoint<TService, TArg>
    {
        public CallEndpoint(ServiceLink link, ICallSchema schema)
            : base(link, schema)
        {
        }

        private CallEndpoint(ServiceLink link, ICallSchema schema, IReadOnlyDictionary<string, object> store) 
            : base(link, schema, store)
        {
        }

        ICallSchema IActionClient<TArg>.Schema => Schema;


        ICallSchema IActionServer<TArg>.Schema => Schema;
        

        public ushort PrefetchCount() => GetParameter(nameof(PrefetchCount), (ushort) 1);
        public ICallEndpoint<TService, TArg> PrefetchCount(ushort value)
            => new CallEndpoint<TService, TArg>(Link, Schema, SetParameter(nameof(PrefetchCount), value));
        
        public TimeSpan Timeout() => GetParameter(nameof(Timeout), TimeSpan.FromMinutes(10));
        public ICallEndpoint<TService, TArg> Timeout(TimeSpan value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Schema, SetParameter(nameof(Timeout), value));
        }
        
        public TimeSpan? ResponseQueueExpires() => GetParameter(nameof(ResponseQueueExpires), TimeSpan.FromHours(1));
        public ICallEndpoint<TService, TArg> ResponseQueueExpires(TimeSpan? value)
        {
            if(value != null &&  value.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("Timeout must be positive time span");
            return new CallEndpoint<TService, TArg>(Link, Schema, SetParameter(nameof(ResponseQueueExpires), value));
        }
        

        public IDisposable Process(Func<TArg, CancellationToken, Task> processor)
        {
            Log.Trace($"{nameof(Process)} enter");
            try
            {
                var queue = Schema.RequestQueue();
                var consumerBuilder =
                    Utils.CreateConsumerBuilder(Link, Schema.Exchange(),
                        false, false, queue.Name, false, null, null, false,
                        PrefetchCount(),
                        new QueueParameters().Durable(queue.Durable).AutoDelete(queue.AutoDelete),
                        new[] {Schema.RoutingKey()}, true);

                var publisher = Utils.CreateProducer(Link, Schema.ResponseExchange(), Schema.ContentType(),
                    false, false);
                consumerBuilder.Handler(async msg =>
                {
                    var log = Log.With("@msg", msg);
                    log.Trace("Message receiving");
                    try
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
                            var answer = new LinkPublishMessage<byte[]>(
                                Link.PayloadManager.Serialize(Schema.ContentType(), new RpcOk(), props),
                                props,
                                new LinkPublishProperties
                                {
                                    RoutingKey = msg.Properties.ReplyTo
                                });
                            await publisher.PublishAsync(answer, msg.Cancellation);
                            log.Trace("Receive success");
                        }
                        catch (Exception ex)
                        {
                            if (tsk.IsCanceled)
                                return LinkConsumerAckStrategy.Requeue;
                            var props = new LinkMessageProperties
                            {
                                CorrelationId = msg.Properties.CorrelationId
                            };
                            var answer = new LinkPublishMessage<byte[]>(Link.PayloadManager.Serialize(
                                    Schema.ContentType(), new RpcFail
                                    {
                                        Kind = ex.GetType().FullName,
                                        Message = ex.Message
                                    }, props),
                                props,
                                new LinkPublishProperties
                                {
                                    RoutingKey = msg.Properties.ReplyTo
                                });
                            await publisher.PublishAsync(answer, msg.Cancellation);
                            log.Trace("Receive success with error", ex);
                        }
                        return LinkConsumerAckStrategy.Ack;
                    }
                    catch (Exception ex)
                    {
                        if(ex.IsCancellation())
                            log.Info("Cancelled");
                        else
                            log.Error("Message receive", ex);
                        throw;
                    }
                });

                return consumerBuilder.Build();
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Process)} error", ex);
                throw;
            }
        }
        
        private async Task Call(TArg arg, CancellationToken token, TimeSpan timeout)
        {
            var log = Log.With("@message", arg);
            log.Trace($"{nameof(Call)} enter");
            try
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
                    var queueName = $"{Schema.Service.Owner}.{Schema.ResponseExchange()}.{Guid.NewGuid():D}";
                    var consumer = Link.GetOrAddConsumer(Schema.ResponseExchange().Name ?? "",
                        () => new RpcConsumer(Link, Utils.CreateConsumerBuilder(Link, Schema.ResponseExchange(),
                            true, false, queueName, false, null, null, false, PrefetchCount(),
                            new QueueParameters().Expires(ResponseQueueExpires()),
                            new[] {queueName}, true), queueName));
                    await consumer.WaitReadyAsync(token);
                    var props = new LinkMessageProperties
                    {
                        CorrelationId = Guid.NewGuid().ToString("D"),
                        ReplyTo = queueName
                    };
                    if (messageTtl != null)
                        props.Expiration = messageTtl.Value;
                    var waiter = consumer.WaitFor<RpcOk>(props.CorrelationId, token);
                    var producer =
                        Utils.CreateProducer(Link, Schema.Exchange(), Schema.ContentType(), true);

                    var request = new LinkPublishMessage<byte[]>(
                        Link.PayloadManager.Serialize(Schema.ContentType(), arg, props),
                        props, new LinkPublishProperties
                        {
                            RoutingKey = Schema.RoutingKey()
                        });
                    await producer.PublishAsync(request, token);
                    await waiter;
                    log.Trace($"{nameof(Call)} success");
                }
                finally
                {
                    source?.Dispose();
                }
            }
            catch (Exception ex)
            {
                if(ex.IsCancellation())
                    log.Info($"{nameof(Call)} cancelled");
                else
                    log.Error($"{nameof(Call)} error", ex);
                throw;
            }
        }

        public Task Call(TArg arg, CancellationToken token)
            => Call(arg, token, Timeout());

        public Task Call(TArg arg, TimeSpan timeout)
            => Call(arg, CancellationToken.None, timeout);
    }
}