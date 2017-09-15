using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface)]
    [ConfigRegister]
    public class OwnerAttribute : Attribute
    {
        public string OwnerName { get; }

        public OwnerAttribute(string ownerName)
        {
            OwnerName = ownerName;
        }
    }
}