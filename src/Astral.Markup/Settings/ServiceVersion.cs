using System;

namespace Astral.Settings
{
    public sealed class ServiceVersion : Fact<Version>
    {
        public ServiceVersion(Version value) : base(value)
        {
        }
    }
}