using LanguageExt;

namespace Astral.Configuration
{
    public class ServiceName : NewType<ServiceName, string>
    {
        public ServiceName(string value) : base(value)
        {
        }
    }
}