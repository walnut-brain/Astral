using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class IsDurableExchange : Fact<bool>
    {
        public IsDurableExchange(bool value) : base(value)
        {
        }
    }
}