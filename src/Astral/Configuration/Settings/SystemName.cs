using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class SystemName : NewType<SystemName, string>
    {
        public SystemName(string value) : base(value)
        {
        }
    }
}