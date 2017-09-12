using System;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceVersionSetting : Fact<Version>
    {
        public ServiceVersionSetting(Version value) : base(value)
        {
        }
    }
}