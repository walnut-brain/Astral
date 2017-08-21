using System;
using Astral.Predicates;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class DeliveryLeaseTime : NewType<DeliveryLeaseTime, TimeSpan, PositiveTimeSpan>
    {
        public DeliveryLeaseTime(TimeSpan value) : base(value)
        {
        }
    }
}