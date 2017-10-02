using System;
using System.Threading;

namespace Astral.Disposables
{
    public class CancellationDisposable : IDisposed
    {
        private readonly CancellationTokenSource _source;
        private readonly IDisposed _disposed;

        public CancellationDisposable(CancellationTokenSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _disposed = Disposable.Create(() =>
            {
                _source.Cancel();
                _source.Dispose();
            });
        }

        public CancellationDisposable() : this(new CancellationTokenSource())
        {
        }

        public CancellationToken Token => _source.Token;

        public bool IsDisposed => _disposed.IsDisposed;

        public void Dispose() => _disposed.Dispose();

    }
}