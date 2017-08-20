using LanguageExt;
using Polly;

namespace Astral.Configuration
{
    public class DeliveryPolicy : NewType<DeliveryPolicy, Policy>
    {
        public DeliveryPolicy(Policy value) : base(value)
        {
        }
    }
}