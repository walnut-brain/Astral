using System;

namespace Astral.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EndpointAttribute : Attribute
    {
        public EndpointAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}