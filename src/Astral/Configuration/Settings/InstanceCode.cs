using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class InstanceCode : NewType<InstanceCode, string>
    {
        public InstanceCode(string value) : base(value)
        {
        }
    }
}