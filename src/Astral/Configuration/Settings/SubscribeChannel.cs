using Astral.Transport;

namespace Astral.Configuration.Settings
{
    public sealed class SubscribeChannel : Fact<ChannelKind>
    {
        public SubscribeChannel(ChannelKind value) : base(value)
        {
        }
    }
}