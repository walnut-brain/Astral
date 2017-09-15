using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseExchangeDeclarePassive : Fact<bool>
    {
        public ResponseExchangeDeclarePassive(bool value) : base(value)
        {
        }
    }
}