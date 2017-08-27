using Astral.Delivery;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class AfterAnswerDelivery : NewType<AfterAnswerDelivery, ReleaseAction>
    {
        public AfterAnswerDelivery(ReleaseAction value) : base(value)
        {
        }
    }
}