using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class OwnerAttribute : Attribute, IConfigAttribute
    {
        public string OwnerName { get; }

        public OwnerAttribute(string ownerName)
        {
            OwnerName = ownerName;
        }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
            => new Fact[] {new ServiceOwner(OwnerName)};
    }
}