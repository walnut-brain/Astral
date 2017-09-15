using Astral;

namespace RabbitLink.Astral.Settings
{
    public sealed class IsDurableResponseExchange : Fact<bool>
    {
        public IsDurableResponseExchange(bool value) : base(value)
        {
        }
    }
}