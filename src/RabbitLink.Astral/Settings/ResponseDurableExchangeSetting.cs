using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseDurableExchangeSetting : Fact<bool>
    {
        public ResponseDurableExchangeSetting(bool value) : base(value)
        {
        }
    }
}