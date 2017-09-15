using System;
using System.Collections.Generic;
using System.Threading;

namespace Lawium
{
    
    /// <inheritdoc />
    /// <summary>
    /// Blocked disposable dictionary
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    public class BlockedDisposableDictionary<TKey, TValue> : IDisposable
    {
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Create empty
        /// </summary>
        public BlockedDisposableDictionary()
        {
        }

        /// <summary>
        /// Create prefilled
        /// </summary>
        /// <param name="from">prefill from</param>
        public BlockedDisposableDictionary(IDictionary<TKey, TValue> from)
        {
            _dictionary = new Dictionary<TKey, TValue>(from);
        }

        private void CheckDisposed()
        {
            if(Disposed)
                throw new ObjectDisposedException(nameof(BlockedDisposableDictionary<TKey, TValue>));
        }

        /// <summary>
        /// Is dictionary disposed
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// get or add key value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="factory">value factory</param>
        /// <returns>value</returns>
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

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Add(TKey key, TValue value)
        {
            CheckDisposed();
            _locker.EnterWriteLock();
            try
            {
                CheckDisposed();
                _dictionary.Add(key, value);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        
        /// <inheritdoc />
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