using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class EndpointName : NewType<EndpointName, string>
    {
        public EndpointName(string value) : base(value)
        {
        }
    }
}