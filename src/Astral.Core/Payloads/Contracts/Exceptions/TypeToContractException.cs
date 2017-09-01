using System;

namespace Astral.Payloads.Contracts
{
    public class TypeToContractException : ContractResolutionException
    {
        public TypeToContractException(Type type)
            : this(type, $"Cannot determine contract name of {type}")
        {
        }

        public TypeToContractException(Type type, string message) : base(message)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}