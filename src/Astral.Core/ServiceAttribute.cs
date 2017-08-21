using System;

namespace Astral.Core
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string version, string name = null)
        {
            Version = Version.Parse(version);
            Name = name;
        }

        public Version Version { get; }
        public string Name { get; }
    }
}