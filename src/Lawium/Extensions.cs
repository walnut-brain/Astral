using System;
using LanguageExt;

namespace Lawium
{
    public static class Extensions
    {
        public static Option<T> TryGet<T>(this LawBook book)
            => book.TryGet(typeof(T)).Unbox<T>();

        public static T Get<T>(this LawBook book)
            => book.TryGet<T>().Unwrap($"Value for type {typeof(T)} not found");


        internal static T Unwrap<T>(this Option<T> option, string message = null)
            => option.IfNone(() => throw new InvalidOperationException(message ?? $"Unwrap None of type {typeof(T)}"));

        internal static Option<T> Unbox<T>(this Option<object> option) 
            => option.Bind(p => p is T v ? Prelude.Some(v) : Prelude.None);                
    }
}