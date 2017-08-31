using System;
using Astral.Markup;
using Astral.Predicates;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryReserveTime : Fact<TimeSpan, PositiveTimeSpan>
    {
        public DeliveryReserveTime(TimeSpan value) : base(value)
        {
        }
    }
}