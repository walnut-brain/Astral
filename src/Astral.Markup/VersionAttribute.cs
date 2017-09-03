using System;
using System.Reflection;
using Astral.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class VersionAttribute : Attribute, IAstralAttribute
    {
        public VersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }

        public Version Version { get; }

        public object GetConfigElement(MemberInfo applyedTo)
        {
            if (applyedTo is TypeInfo ti && ti.IsInterface)
                return new ServiceVersion(Version);
            throw new NotImplementedException();
        }
    }
}