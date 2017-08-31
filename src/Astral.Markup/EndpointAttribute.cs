using System;

namespace Astral.Markup
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