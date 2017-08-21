using System;
using Astral.Predicates;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class DeliveryReserveTime : NewType<DeliveryReserveTime, TimeSpan, PositiveTimeSpan>
    {
        public DeliveryReserveTime(TimeSpan value) : base(value)
        {
        }
    }
    
    
}