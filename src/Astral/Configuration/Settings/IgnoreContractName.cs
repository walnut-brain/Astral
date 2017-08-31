using Astral.Markup;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class IgnoreContractName : Fact<bool>
    {
        public IgnoreContractName(bool value) : base(value)
        {
        }
    }
}