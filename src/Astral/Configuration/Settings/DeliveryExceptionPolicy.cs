using Polly;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryExceptionPolicy : Fact<Policy>
    {
        public DeliveryExceptionPolicy(Policy value) : base(value)
        {
        }
    }
}