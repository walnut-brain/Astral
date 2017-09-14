using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ExchangeNameSetting : Fact<string>
    {
        public ExchangeNameSetting(string value) : base(value)
        {
        }
    }
}