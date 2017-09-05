using System;

namespace Astral.Payloads.DataContracts
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