using System;
using System.Reactive.Disposables;

namespace Astral.Utils
{
    internal class DisposableValue<T> : IDisposable
    {
        private readonly Lazy<T> _lazy;
        private readonly T _value;
        private readonly IDisposable _disposable;

        public DisposableValue(T value, bool isOwned)
        {
            _value = value;
            _lazy = null;
            if(isOwned && _value is IDisposable d)
                _disposable = Disposable.Create(() => d.Dispose());
        }

        public DisposableValue(Lazy<T> lazy, bool isOwned)
        {
            _disposable = new CompositeDisposable();
            if(isOwned)
                _disposable = Disposable.Create(() =>
                {
                    if(lazy.IsValueCreated && lazy.Value is IDisposable d)
                        d.Dispose();
                });
        }

        public T Value => _lazy == null ? _value : _lazy.Value;

        
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}