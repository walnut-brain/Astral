using Astral.Delivery;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class AfterDelivery : NewType<AfterDelivery, OnDeliverySuccess>
    {
        public AfterDelivery(OnDeliverySuccess value) : base(value)
        {
        }
    }
}