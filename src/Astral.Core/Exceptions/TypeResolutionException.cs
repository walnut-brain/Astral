using System;

namespace Astral.Core
{
    public class TypeResolutionException : DataContractResolutionException
    {
        public TypeResolutionException(Type type) : base($"Cannot determine contract name of {type}")
        {
        }
    }
}