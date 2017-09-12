using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class InstanceCodeSetting : Fact<string>
    {
        public InstanceCodeSetting(string value) : base(value)
        {
        }
    }
}