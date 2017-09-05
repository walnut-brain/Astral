using System;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceVersion : Fact<Version>
    {
        public ServiceVersion(Version value) : base(value)
        {
        }
    }
}