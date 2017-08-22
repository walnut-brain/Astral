using System;

namespace Astral.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ContractAttribute : Attribute
    {
        public ContractAttribute(string name)
        {
            Name = name;
            
        }

        public string Name { get; }
        
    }
}