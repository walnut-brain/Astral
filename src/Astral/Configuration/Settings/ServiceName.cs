using Astral.Markup;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceName : Fact<string>
    {
        public ServiceName(string value) : base(value)
        {
        }
    }
}