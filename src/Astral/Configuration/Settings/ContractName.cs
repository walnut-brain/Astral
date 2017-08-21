using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ContractName : NewType<ContractName, string>
    {
        public ContractName(string value) : base(value)
        {
        }
    }
}