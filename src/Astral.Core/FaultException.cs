using System;

namespace Astral
{
    public class FaultException : Exception
    {
        public FaultException(string code)
        {
            Code = code;
        }

        public FaultException(string message, string code) : base(message)
        {
            Code = code;
        }

        public FaultException(string message, Exception innerException, string code) : base(message, innerException)
        {
            Code = code;
        }

        public string Code { get; }
    }
}