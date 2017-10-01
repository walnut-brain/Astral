using System;

namespace Astral.Markup
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public VersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }

        public Version Version { get; }
        
    }
}