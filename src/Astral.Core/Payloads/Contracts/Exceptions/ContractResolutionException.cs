using System;

namespace Astral.Payloads.Contracts
{
    public class ContractResolutionException : PermanentException
    {
        public ContractResolutionException()
        {
        }

        public ContractResolutionException(string message) : base(message)
        {
        }

        public ContractResolutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}