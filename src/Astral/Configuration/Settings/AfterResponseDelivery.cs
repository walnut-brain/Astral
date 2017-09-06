using Astral.Deliveries;
using Lawium;


namespace Astral.Configuration.Settings
{
    public sealed class AfterResponseDelivery : Fact<DeliveryOnSuccess>
    {
        public AfterResponseDelivery(DeliveryOnSuccess value) : base(value)
        {
        }
    }
}