﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using RabbitLink.Builders;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace RabbitLink.Services.Internals
{
    internal class RpcConsumer : IDisposable
    {
        public string QueueName { get; }
        private readonly ServiceLink _link;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<ILinkConsumedMessage<byte[]>>>
            _subscribers = new ConcurrentDictionary<string, TaskCompletionSource<ILinkConsumedMessage<byte[]>>>();

        private readonly ILinkConsumer _consumer;

        public RpcConsumer(ServiceLink link, ILinkConsumerBuilder builder, string queueName)
        {
            QueueName = queueName;
            _link = link;
            _consumer = builder.Handler(Handle).Build();
        }

        private Task<LinkConsumerAckStrategy> Handle(ILinkConsumedMessage<byte[]> message)
        {
            if (_subscribers.TryGetValue(message.Properties.CorrelationId, out var source))
            {
                source.TrySetResult(message);
                return Task.FromResult(LinkConsumerAckStrategy.Ack);
            }
            return Task.FromResult(LinkConsumerAckStrategy.Nack);
        }

        public Task WaitReadyAsync(CancellationToken token) => _consumer.WaitReadyAsync(token);

        public async Task<T> WaitFor<T>(string correlationId, CancellationToken token)
        {
            var taskSource = new TaskCompletionSource<ILinkConsumedMessage<byte[]>>();
            _subscribers.TryAdd(correlationId, taskSource);

            try
            {
                token.Register(() => taskSource.TrySetCanceled(token));
                var msg = await taskSource.Task;
                var obj = _link.PayloadManager.Deserialize(msg, typeof(T));
                switch (obj)
                {
                    case Exception ex:
                        throw ex;
                    case RpcFail fail:
                        throw new RpcFailException(fail.Message, fail.Kind);
                    case T t:
                        return t;
                }
                throw new InvalidCastException($"Invalid message received {obj?.GetType()}");
            }
            finally
            {
                _subscribers.TryRemove(correlationId, out var _);
            }
        }

        public void Dispose()
        {
            _consumer.Dispose();
            while (!_subscribers.IsEmpty)
            {
                var code = _subscribers.Keys.FirstOrDefault();
                if (code != null && _subscribers.TryRemove(code, out var source))
                    source.TrySetCanceled();
            }
        }
    }
}