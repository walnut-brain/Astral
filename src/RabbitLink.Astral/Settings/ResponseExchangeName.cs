using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseExchangeName : Fact<string>
    {
        public ResponseExchangeName(string value) : base(value)
        {
        }
    }
}