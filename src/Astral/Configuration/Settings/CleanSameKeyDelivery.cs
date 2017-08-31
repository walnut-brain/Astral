using Astral.Markup;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class CleanSameKeyDelivery : Fact<bool>
    {
        public CleanSameKeyDelivery(bool value) : base(value)
        {
        }
    }
}