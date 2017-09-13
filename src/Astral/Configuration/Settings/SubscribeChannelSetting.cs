using Astral.Transport;

namespace Astral.Configuration.Settings
{
    public sealed class SubscribeChannelSetting : Fact<ChannelKind>
    {
        public SubscribeChannelSetting(ChannelKind value) : base(value)
        {
        }
    }
}