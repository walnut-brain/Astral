using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseConfirmsModeSetting : Fact<bool>
    {
        public ResponseConfirmsModeSetting(bool value) : base(value)
        {
        }
    }
}