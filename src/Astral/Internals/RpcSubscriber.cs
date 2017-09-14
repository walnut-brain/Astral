using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Astral.Payloads;
using Astral.Transport;

namespace Astral.Internals
{
    internal class RpcSubscriber : IDisposable
    {
        private readonly ICancelable _disposable;
        private readonly ConcurrentDictionary<string, Action<Payload<byte[]>,MessageContext>> _listeners = new ConcurrentDictionary<string, Action<Payload<byte[]>,MessageContext>>();

        public RpcSubscriber(Subscribable subscribable)
        {
            _disposable = new CompositeDisposable(subscribable(RawHandle));
        }


        private Task<Acknowledge> RawHandle(Payload<byte[]> payload, MessageContext context, CancellationToken token)
        {
            if (_listeners.TryRemove(context.RequestId, out var handler))
            {
                Task.Run(() => handler(payload, context));
                return Task.FromResult(Acknowledge.Ack);
            }
            return Task.FromResult(Acknowledge.Nack);
        }

        public Task<Payload<byte[]>> AnswerAsync(string requestId, CancellationToken token)
        {
            if(_disposable.IsDisposed) throw new ObjectDisposedException(nameof(RpcSubscriber));
            var taskSource = new TaskCompletionSource<Payload<byte[]>>();
            _listeners.TryAdd(requestId, (payload, ctx) => taskSource.TrySetResult(payload));
            token.Register(() =>
            {
                _listeners.TryRemove(requestId, out var _);
                taskSource.TrySetCanceled();
            });
            return taskSource.Task;
        }
        
        
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}