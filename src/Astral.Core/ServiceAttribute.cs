using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute, IConfigAttribute
    {
        public ServiceAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
            => new Fact[] {new ServiceName(Name)};
    }
}