using LanguageExt;

namespace Astral.Configuration
{
    public class EndpointName : NewType<EndpointName, string>
    {
        public EndpointName(string value) : base(value)
        {
        }
    }
}