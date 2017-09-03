using System;
using System.Reflection;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute, IAstralAttribute
    {
        public ServiceAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public object GetConfigElement(MemberInfo applyedTo)
            => new ServiceName(Name);
    }
}