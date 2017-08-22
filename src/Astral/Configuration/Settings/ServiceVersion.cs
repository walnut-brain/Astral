using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ServiceVersion : NewType<ServiceVersion, Version>
    {
        public ServiceVersion(Version value) : base(value)
        {
        }
    }
}