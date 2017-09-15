using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Schema;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Property)]
    [ConfigRegister]
    public class EndpointAttribute : Attribute
    {
        public EndpointAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

    }
}