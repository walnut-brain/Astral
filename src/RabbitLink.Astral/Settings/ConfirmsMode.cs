using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ConfirmsMode : Fact<bool>
    {
        public ConfirmsMode(bool value) : base(value)
        {
        }
    }
}