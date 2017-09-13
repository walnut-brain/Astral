using System;
using Astral.Deliveries;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryReplayToSetting :Fact<ChannelKind> 
    {
        public DeliveryReplayToSetting(ChannelKind value) : base(value)
        {
            if(!(value is ChannelKind.IDeliveryReply))
                throw new ArgumentOutOfRangeException($"Channel must support delivery reply");
        }
    }
}