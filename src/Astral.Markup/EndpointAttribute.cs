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

        public (Type, object) GetConfigElement(MemberInfo applyedTo)
            => (typeof(EndpointName), new EndpointName(Name));

    }
}