using System;
using System.Threading;
using Astral.Markup;
using LanguageExt.TypeClasses;

namespace Astral.Predicates
{
    public struct PositiveTimeSpan : Pred<TimeSpan>, IPredicate<TimeSpan>
    {
        public bool True(TimeSpan value)
        {
            return value > TimeSpan.Zero || value == Timeout.InfiniteTimeSpan;
        }

        (bool, string) IPredicate<TimeSpan>.True(TimeSpan value)
        {
            var result = True(value);
            return (result, result ? null : "Value must be positive or Timeout.InfiniteTimeSpan");
        }
    }
}