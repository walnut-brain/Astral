using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FunEx
{
    public static class Extensions
    {
        public static ImmutableArray<TResult> Map<T, TResult>(this ImmutableArray<T> array,
            Func<T, TResult> mapper)
            => array.Select(mapper).ToImmutableArray();

        public static IEnumerable<T> AsEnumerable<T>(this T value) => new[] {value};
        
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> value) => value.FirstOrDefault().ToOption();

        public static Result<T> FirstOrError<T>(this IEnumerable<Result<T>> en, Exception onNotFound)
        {
            foreach (var result in en)
            {
                if (result.IsOk)
                    return result;
            }
            return onNotFound;
        }

        public static Exception FlatCombine(this Exception ex1, Exception ex2)
        {
            if (ex1 == null) throw new ArgumentNullException(nameof(ex1));
            if (ex2 == null) throw new ArgumentNullException(nameof(ex2));
            switch (ex1)
            {
                case AggregateException aex1:
                    switch (ex2)
                    {
                        case AggregateException aex2:
                            return new AggregateException(aex1.InnerExceptions.Union(aex2.InnerExceptions));
                        default:
                            return new AggregateException(aex1.InnerExceptions.Append(ex2));
                    }
                default:
                    switch (ex2)
                    {
                        case AggregateException aex2:
                            return new AggregateException(aex2.InnerExceptions.Prepend(ex1));
                        default:
                            return new AggregateException(ex1, ex2);
                    }
            }
        }
    }
}