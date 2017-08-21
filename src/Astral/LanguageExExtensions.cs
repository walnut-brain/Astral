using System;
using LanguageExt;

namespace Astral
{
    public static class LanguageExExtensions
    {
        public static T Unwrap<T>(this Option<T> option, Func<Option<T>, string> onError)
            => option.Match(p => p, () => throw new InvalidOperationException(onError(option)));

        public static T Unwrap<T>(this Option<T> option, Func<string> onError)
            => Unwrap(option, p => onError());

        public static T Unwrap<T>(this Option<T> option, string onError)
            => Unwrap(option, p => onError);

        public static T Unwrap<T>(this Option<T> option)
            => Unwrap(option, p => $"Unwrap None of type {typeof(T)}");

        public static Option<T> UnboxUnsafe<T>(this Option<object> option)
            => option.Map(p => (T) p);

        public static Option<T> Unbox<T>(this Option<object> option) 
            => option.Bind(p => p is T v ? LanguageExt.Prelude.Some(v) : LanguageExt.Prelude.None);

        public static Option<object> Box<T>(this Option<T> option)
            => option.Map(p => (object) p);
    }
}