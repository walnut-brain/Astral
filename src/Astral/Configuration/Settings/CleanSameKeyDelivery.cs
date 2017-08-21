using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class CleanSameKeyDelivery : NewType<CleanSameKeyDelivery, bool>
    {
        public CleanSameKeyDelivery(bool value) : base(value)
        {
        }
    }
}