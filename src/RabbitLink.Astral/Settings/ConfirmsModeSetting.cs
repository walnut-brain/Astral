using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ConfirmsModeSetting : Fact<bool>
    {
        public ConfirmsModeSetting(bool value) : base(value)
        {
        }
    }
}