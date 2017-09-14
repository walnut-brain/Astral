using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseExchangeNameSetting : Fact<string>
    {
        public ResponseExchangeNameSetting(string value) : base(value)
        {
        }
    }
}