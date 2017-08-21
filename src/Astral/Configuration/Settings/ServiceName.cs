using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ServiceName : NewType<ServiceName, string>
    {
        public ServiceName(string value) : base(value)
        {
        }
    }
}