using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    /// <summary>
    /// Result of law inference
    /// </summary>
    public class LawBook<T>
    {
        private readonly string _path;
        private readonly IReadOnlyCollection<Law<T>> _laws;
        private readonly IReadOnlyDictionary<Type, T> _facts;
        private readonly Dictionary<object, Task<LawBook<T>>> _subBooks = new Dictionary<object, Task<LawBook<T>>>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        internal LawBook(
            ILoggerFactory loggerFactory,
            string path, 
            IReadOnlyCollection<Law<T>> laws, 
            IReadOnlyDictionary<Type, T> facts,
            IReadOnlyDictionary<object, LawBook<T>> subBooks)
        {
            LoggerFactory = loggerFactory;
            _path = path;
            _laws = laws;
            _facts = facts;
            foreach (var book in subBooks)
            {
                _subBooks.Add(book.Key, Task.FromResult(book.Value));
            }
            
        }

        /// <summary>
        /// logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Untyped read value
        /// </summary>
        /// <param name="type">key type</param>
        /// <returns>Some value or none</returns>
        public Option<T> TryGet(Type type)
            => _facts.TryGetValue(type);
        
        /// <summary>
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <returns>Some with value when available, else None</returns>
        public Option<TKey> TryGet<TKey>()
            where TKey : T
            => TryGet(typeof(TKey)).OfType<TKey>();
        
        /// <summary>
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="value">value or default(T)</param>
        /// <returns>true if found</returns>
        public bool TryGet<TKey>(out TKey value)
            where TKey : T
        {
            var result = TryGet<TKey>().Match(p => (true, p), () => (false, default(TKey)));
            value = result.Item2;
            return result.Item1;
        }
        
        /// <summary>
        /// Get value from book or throw
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <returns>value</returns>
        public TKey Get<TKey>() where TKey : T => TryGet<TKey>().Unwrap();

        /// <summary>
        /// Dynamic add chield law book
        /// </summary>
        /// <param name="key">chield book key</param>
        /// <param name="onBuild">on build parameters</param>
        /// <returns>awaitable result of build law book</returns>
        public Task<LawBook<T>> GetOrAddSubBook(object key, Action<LawBookBuilder<T>> onBuild = null)
        {
            onBuild = onBuild ?? (_ => {});
            
            _locker.EnterReadLock();
            try
            {
                if(_subBooks.TryGetValue(key, out var task))
                    return task;
            }
            finally 
            {
                _locker.ExitReadLock();
            }
            _locker.EnterWriteLock();
            try
            {
                if (_subBooks.TryGetValue(key, out var task))
                    return task;
                var builder = new LawBookBuilder<T>(LoggerFactory, () => _laws, _path + "/" + key);
                onBuild(builder);
                task = Task.Factory.StartNew(() => builder.Build(), TaskCreationOptions.LongRunning);
                _subBooks.Add(key, task);
                return task;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

    }
}