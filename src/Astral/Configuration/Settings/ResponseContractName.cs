using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ResponseContractName : NewType<ResponseContractName, string>
    {
        public ResponseContractName(string value) : base(value)
        {
        }
    }
}