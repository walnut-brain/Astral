using Astral.Deliveries;
using Lawium;


namespace Astral.Configuration.Settings
{
    public sealed class AfterResponseDelivery : Fact<OnDeliverySuccess>
    {
        public AfterResponseDelivery(OnDeliverySuccess value) : base(value)
        {
        }
    }
}