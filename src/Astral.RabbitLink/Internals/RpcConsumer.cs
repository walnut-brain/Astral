using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astral.Liaison;
using Astral.Schema;
using RabbitLink.Builders;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace Astral.RabbitLink.Internals
{
    internal class RpcConsumer : IDisposable
    {
        public string QueueName { get; }
        private readonly ServiceLink _link;
        private bool _isDisposed;
        private ReaderWriterLockSlim DisposeLock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

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

        private void GuardDispose(Action action)
        {
            DisposeLock.EnterReadLock();
            try
            {
                if(_isDisposed) throw new ObjectDisposedException(GetType().Name);
                action();
            }
            finally
            {
                DisposeLock.ExitReadLock();
            }
        }
        
        public async Task<T> WaitFor<T>(string correlationId, CancellationToken token, IServiceSchema schema)
        {
            var taskSource = new TaskCompletionSource<ILinkConsumedMessage<byte[]>>();
            GuardDispose(() => _subscribers.TryAdd(correlationId, taskSource));

            try
            {
                token.Register(() => taskSource.TrySetCanceled(token));
                var msg = await taskSource.Task;
                var obj = _link.PayloadManager.Deserialize<T>(msg, schema.Types);
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
            DisposeLock.EnterWriteLock();
            try
            {
                if (_isDisposed) return;
                _isDisposed = true;
                _consumer.Dispose();
                while (!_subscribers.IsEmpty)
                {
                    var code = _subscribers.Keys.FirstOrDefault();
                    if (code != null && _subscribers.TryRemove(code, out var source))
                        source.TrySetCanceled();
                }
            }
            finally
            {
                DisposeLock.ExitWriteLock();
            }
        }
    }
}