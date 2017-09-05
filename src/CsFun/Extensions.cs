using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CsFun
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
                if (result.IsSuccess)
                    return result;
            }
            return onNotFound.ToFail<T>();
        }
    }
}