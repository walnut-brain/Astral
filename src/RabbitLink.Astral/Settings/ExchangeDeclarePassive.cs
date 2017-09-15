using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ExchangeDeclarePassive : Fact<bool>
    {
        public ExchangeDeclarePassive(bool value) : base(value)
        {
        }
    }
}