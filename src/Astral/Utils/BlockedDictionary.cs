using System;
using System.Collections.Generic;
using System.Threading;

namespace Astral.Utils
{
    public class BlockedDisposableDictionary<TKey, TValue> : IDisposable
    {
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        private void CheckDisposed()
        {
            if(Disposed)
                throw new ObjectDisposedException(nameof(BlockedDisposableDictionary<TKey, TValue>));
        }

        public bool Disposed { get; private set; }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            CheckDisposed();
            _locker.EnterReadLock();
            try
            {
                CheckDisposed();
                if (_dictionary.TryGetValue(key, out var result))
                    return result;
            }
            finally
            {
                _locker.ExitReadLock();
            }
            CheckDisposed();
            _locker.EnterWriteLock();
            try
            {
                CheckDisposed();
                if (!_dictionary.TryGetValue(key, out var result))
                {
                    result = factory(key);
                    _dictionary.Add(key, result);
                }
                return result;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            if(Disposed) return;
            _locker.EnterWriteLock();
            try
            {
                if (Disposed) return;
                Disposed = true;
                foreach (var value in _dictionary.Values)
                {
                    if(value is IDisposable d)
                        d.Dispose();
                }
                _dictionary.Clear();
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }
    }

    
}