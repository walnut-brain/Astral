using System;
using LanguageExt.TypeClasses;

namespace Astral.Predicates
{
    public struct PositiveTimeSpan : Pred<TimeSpan>
    {
        public bool True(TimeSpan value)
            => value > TimeSpan.Zero;
    }
}