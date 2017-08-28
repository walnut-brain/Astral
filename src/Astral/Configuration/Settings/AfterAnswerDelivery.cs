using Astral.Delivery;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class AfterAnswerDelivery : NewType<AfterAnswerDelivery, OnDeliverySuccess>
    {
        public AfterAnswerDelivery(OnDeliverySuccess value) : base(value)
        {
        }
    }
}