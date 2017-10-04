using System;
using System.Threading;

namespace Astral.Disposables
{
    public class DelegateDisposable : IDisposed
    {
        private Action _onDispose;
        private int _isDisposed;

        public DelegateDisposable(Action onDispose)
        {
            _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
        }

        public bool IsDisposed => Interlocked.CompareExchange(ref _isDisposed, 0, 0) == 0;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            {
                _onDispose();
                _onDispose = null;
            }
        }
    }
}