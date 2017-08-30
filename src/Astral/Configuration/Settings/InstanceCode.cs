using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class InstanceCode : Fact<string>
    {
        public InstanceCode(string value) : base(value)
        {
        }
    }
}