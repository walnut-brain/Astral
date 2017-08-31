using Astral.Delivery;
using Astral.Markup;
using Lawium;


namespace Astral.Configuration.Settings
{
    public sealed class AfterAnswerDelivery : Fact<OnDeliverySuccess>
    {
        public AfterAnswerDelivery(OnDeliverySuccess value) : base(value)
        {
        }
    }
}