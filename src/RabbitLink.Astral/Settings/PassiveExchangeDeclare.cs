using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class PassiveExchangeDeclareSetting : Fact<bool>
    {
        public PassiveExchangeDeclareSetting(bool value) : base(value)
        {
        }
    }
}