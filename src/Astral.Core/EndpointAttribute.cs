using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Schema;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EndpointAttribute : Attribute, IConfigAttribute
    {
        public EndpointAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
            => new Fact[] {  new EndpointName(Name) };
    }
}