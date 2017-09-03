using System;
using System.Reflection;
using Astral.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EndpointAttribute : Attribute, IAstralAttribute
    {
        public EndpointAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public object GetConfigElement(MemberInfo applyedTo) => new EndpointName(Name);

    }
}