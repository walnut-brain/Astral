using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class IgnoreContractName : NewType<IgnoreContractName, bool>
    {
        public IgnoreContractName(bool value) : base(value)
        {
        }
    }
}