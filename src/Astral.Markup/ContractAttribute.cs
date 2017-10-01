using System;

namespace Astral.Markup
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