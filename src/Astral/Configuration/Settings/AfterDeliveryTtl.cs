using System;
using Astral.Predicates;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class AfterDeliveryTtl : NewType<AfterDeliveryTtl, TimeSpan, PositiveTimeSpan>
    {
        public AfterDeliveryTtl(TimeSpan value) : base(value)
        {
        }
    }
}