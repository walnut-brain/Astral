using System;

namespace Astral.Payloads.DataContracts
{
    public class RecursiveResolutionException : TypeEncodingException
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