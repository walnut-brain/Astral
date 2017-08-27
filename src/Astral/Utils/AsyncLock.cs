using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Utils
{
    public class AsyncLock 
    {
        private readonly SemaphoreSlim _locker = new SemaphoreSlim(1);

        public async Task<IDisposable> Take(CancellationToken token)
        {
            await _locker.WaitAsync(token);
            return Disposable.Create(() => _locker.Release());
        }

        public async Task<IDisposable> Take(TimeSpan timeout)
        {
            await _locker.WaitAsync(timeout);
            return Disposable.Create(() => _locker.Release());
        }

        public async Task<IDisposable> Take()
        {
            await _locker.WaitAsync();
            return Disposable.Create(() => _locker.Release());
        }
    }
}