using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class DurableExchangeSetting : Fact<bool>
    {
        public DurableExchangeSetting(bool value) : base(value)
        {
        }
    }
}