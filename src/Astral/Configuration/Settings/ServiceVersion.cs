using System;
using Astral.Markup;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceVersion : Fact<Version>
    {
        public ServiceVersion(Version value) : base(value)
        {
        }
    }
}