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
    public class LawBook
    {
        private readonly string _path;
        private readonly IReadOnlyCollection<Law> _laws;
        private readonly IReadOnlyDictionary<Type, object> _facts;
        private readonly BlockedDisposableDictionary<object, LawBook> _subBooks;

        internal LawBook(
            ILoggerFactory loggerFactory,
            string path, 
            IReadOnlyCollection<Law> laws, 
            IReadOnlyDictionary<Type, object> facts,
            IDictionary<object, LawBook> subBooks)
        {
            LoggerFactory = loggerFactory;
            _path = path;
            _laws = laws;
            _facts = facts;
            _subBooks = new BlockedDisposableDictionary<object, LawBook>(subBooks);
            
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
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <returns>Some with value when available, else None</returns>
        public Option<TKey> TryGet<TKey>()
            => TryGet(typeof(TKey)).OfType<TKey>();
        
        /// <summary>
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="value">value or default(T)</param>
        /// <returns>true if found</returns>
        public bool TryGet<TKey>(out TKey value)
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
        public TKey Get<TKey>() => TryGet<TKey>().Unwrap();

        /// <summary>
        /// Dynamic add chield law book
        /// </summary>
        /// <param name="key">chield book key</param>
        /// <param name="onBuild">on build parameters</param>
        /// <returns>law book</returns>
        public LawBook GetOrAddSubBook(object key, Action<LawBookBuilder> onBuild = null)
        {
            onBuild = onBuild ?? (_ => {});
            
            return _subBooks.GetOrAdd(key, _ => { 
                var builder = new LawBookBuilder(LoggerFactory, () => _laws, _path + "/" + key);
                onBuild(builder);
                return builder.Build();

            });
        }
        
        /// <summary>
        /// Add subbook without saving
        /// </summary>
        /// <param name="onBuild">on build law adding</param>
        /// <returns>new law book</returns>
        public LawBook AddSubBook(Action<LawBookBuilder> onBuild = null)
        {
            onBuild = onBuild ?? (_ => {});
            
            var builder = new LawBookBuilder(LoggerFactory, () => _laws, Guid.NewGuid().ToString("D"));
            onBuild(builder);
            return builder.Build();

        }

    }
}