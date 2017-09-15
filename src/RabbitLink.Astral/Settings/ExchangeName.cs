using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ExchangeName : Fact<string>
    {
        public ExchangeName(string value) : base(value)
        {
        }
    }
}