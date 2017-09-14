using System;
using Astral.Deliveries;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryReplayToSetting :Fact<ChannelKind.DurableChannel> 
    {
        public DeliveryReplayToSetting(ChannelKind.DurableChannel value) : base(value)
        {
            
        }
    }
}