using System;

namespace Astral.Schema.Data
{
    public abstract class NamedTypeDesc : TypeDesc
    {
        private readonly string _contractName;


        protected NamedTypeDesc(Type dotNetType, string name, string contractName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            _contractName = contractName;
            DotNetType = dotNetType;
            Name = name;
        }

        public string Name { get; }

        public override string Contract => _contractName ?? Name;

        public override Type DotNetType { get; }

        public bool HasContract => _contractName != null;

    }
}