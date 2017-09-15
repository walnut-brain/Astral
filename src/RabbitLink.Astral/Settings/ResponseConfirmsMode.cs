using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseConfirmsMode : Fact<bool>
    {
        public ResponseConfirmsMode(bool value) : base(value)
        {
        }
    }
}