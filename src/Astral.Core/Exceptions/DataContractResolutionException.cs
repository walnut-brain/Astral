using System;

namespace Astral.Core
{
    public class DataContractResolutionException : PermanentException
    {
        public DataContractResolutionException()
        {
        }

        public DataContractResolutionException(string message) : base(message)
        {
        }

        public DataContractResolutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}