using System;
using Astral.Deliveries;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryReplayTo :Fact<ChannelKind.DurableChannel> 
    {
        public DeliveryReplayTo(ChannelKind.DurableChannel value) : base(value)
        {
            
        }
    }
}