using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class VersionAttribute : Attribute, IConfigAttribute
    {
        public VersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }

        public Version Version { get; }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
        {
            if (applyedTo is TypeInfo ti && ti.IsInterface)
                return new Fact[] { new ServiceVersion(Version) };
            throw new NotImplementedException();
        }
    }
}