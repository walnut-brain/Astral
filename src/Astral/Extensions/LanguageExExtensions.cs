using System;
using LanguageExt;

namespace Astral
{
    public static class LanguageExExtensions
    {
        public static T Unwrap<T>(this Option<T> option, Func<string> onError)
        {
            return option.IfNone(() => throw new InvalidOperationException(onError()));
        }

        public static T Unwrap<T>(this Option<T> option, string onError)
        {
            return Unwrap(option, () => onError);
        }

        public static T Unwrap<T>(this Option<T> option)
        {
            return Unwrap(option, () => $"Unwrap None of type {typeof(T)}");
        }

        public static T Unwrap<T>(this Result<T> result)
        {
            return result.IfFail(ex => throw ex);
        }

        public static Option<T> UnboxUnsafe<T>(this Option<object> option)
        {
            return option.Map(p => (T) p);
        }

        public static Option<T> Unbox<T>(this Option<object> option)
        {
            return option.Bind(p => p is T v ? Prelude.Some(v) : Prelude.None);
        }

        public static Option<object> Box<T>(this Option<T> option)
        {
            return option.Map(p => (object) p);
        }

        public static Try<T> ToTry<T>(this Option<T> option, Exception ex)
        {
            return option.Match(Prelude.Try, () => Prelude.Try<T>(ex));
        }

        public static Try<T> BindFail<T>(this Try<T> @try, Func<Try<T>> fnc)
        {
            return @try.BiBind(Prelude.Try, _ => fnc());
        }
    }
}