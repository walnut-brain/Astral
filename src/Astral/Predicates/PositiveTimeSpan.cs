using System;
using System.Threading;
using LanguageExt.TypeClasses;

namespace Astral.Predicates
{
    public struct PositiveTimeSpan : Pred<TimeSpan>
    {
        public bool True(TimeSpan value)
        {
            return value > TimeSpan.Zero || value == Timeout.InfiniteTimeSpan;
        }
    }
}