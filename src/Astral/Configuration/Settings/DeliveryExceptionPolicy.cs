using LanguageExt;
using Polly;

namespace Astral.Configuration.Settings
{
    public class DeliveryExceptionPolicy : NewType<DeliveryExceptionPolicy, Policy>
    {
        public DeliveryExceptionPolicy(Policy value) : base(value)
        {
        }
    }
}