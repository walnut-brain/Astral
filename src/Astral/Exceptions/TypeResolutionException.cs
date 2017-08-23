using System;

namespace Astral.Exceptions
{
    public class TypeResolutionException : DataContractResolutionException
    {
        public TypeResolutionException(Type type) : base($"Cannot determine contract name of {type}")
        {
        }

        
    }
}