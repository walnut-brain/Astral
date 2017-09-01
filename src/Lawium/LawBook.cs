﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    /// <summary>
    /// Result of law inference
    /// </summary>
    public class LawBook
    {
        private readonly string _path;
        private readonly IReadOnlyCollection<global::Lawium.Law> _laws;
        private readonly IReadOnlyDictionary<Type, object> _facts;
        private readonly Dictionary<object, Task<LawBook>> _subBooks = new Dictionary<object, Task<LawBook>>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        internal LawBook(
            ILoggerFactory loggerFactory,
            string path, 
            IReadOnlyCollection<global::Lawium.Law> laws, 
            IReadOnlyDictionary<Type, object> facts,
            IReadOnlyDictionary<object, LawBook> subBooks)
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
        public Option<object> TryGet(Type type)
            => _facts.TryGetValue(type);

        /// <summary>
        /// Dynamic add chield law book
        /// </summary>
        /// <param name="key">chield book key</param>
        /// <param name="onBuild">on build parameters</param>
        /// <returns>awaitable result of build law book</returns>
        public Task<LawBook> GetOrAddSubBook(object key, Action<LawBookBuilder> onBuild = null)
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
                var builder = new LawBookBuilder(LoggerFactory, () => _laws, _path + "/" + key);
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