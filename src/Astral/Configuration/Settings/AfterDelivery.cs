using Astral.Delivery;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class AfterDelivery : NewType<AfterDelivery, ReleaseAction>
    {
        public AfterDelivery(ReleaseAction value) : base(value)
        {
        }
    }
}