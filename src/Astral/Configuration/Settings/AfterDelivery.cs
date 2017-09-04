using Astral.Deliveries;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class AfterDelivery : Fact<OnDeliverySuccess>
    {
        public AfterDelivery(OnDeliverySuccess value) : base(value)
        {
        }
    }
}