using System;

namespace Astral.Payloads.DataContracts
{
    public class RecursiveResolutionException : ContractResolutionException
    {
        public RecursiveResolutionException(string contract)
            : base($"Recursive resolution of contract {contract}")
        {
        }

        public RecursiveResolutionException(Type type)
            : base($"Recursive resolution of type {type}")
        {
        }
    }
}