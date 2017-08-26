using System;
using LanguageExt;

namespace Lawium
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="T">key type</typeparam>
        /// <param name="book">law book</param>
        /// <returns>Some with value when available, else None></list></returns>
        public static Option<T> TryGet<T>(this LawBook book)
            => book.TryGet(typeof(T)).Unbox<T>();

        /// <summary>
        /// Try get value from LawBook
        /// </summary>
        /// <typeparam name="T">key type</typeparam>
        /// <param name="book">law book</param>
        /// <param name="value">value or default(T)</param>
        /// <returns>true if found</returns>
        public static bool TryGet<T>(this LawBook book, out T value)
        {
            var result = book.TryGet(typeof(T)).Unbox<T>().Match(p => (true, p), () => (false, default(T)));
            value = result.Item2;
            return result.Item1;
        }

        /// <summary>
        /// Get value from book or throw
        /// </summary>
        /// <typeparam name="T">key type</typeparam>
        /// <param name="book">law book</param>
        /// <returns>value</returns>
        public static T Get<T>(this LawBook book)
            => book.TryGet<T>().Unwrap($"Value for type {typeof(T)} not found");


        internal static T Unwrap<T>(this Option<T> option, string message = null)
            => option.IfNone(() => throw new InvalidOperationException(message ?? $"Unwrap None of type {typeof(T)}"));

        internal static Option<T> Unbox<T>(this Option<object> option) 
            => option.Bind(p => p is T v ? Prelude.Some(v) : Prelude.None);                
    }
}