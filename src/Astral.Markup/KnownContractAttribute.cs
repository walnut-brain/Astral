using System;

namespace Astral.Markup
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class KnownContractAttribute : Attribute
    {
        public KnownContractAttribute(Type type)
        {
            Type = type;
        }

        public KnownContractAttribute(Type type, string contract)
        {
            Type = type;
            Contract = contract;
        }

        public Type Type { get; }
        public string Contract { get; }
    }
}