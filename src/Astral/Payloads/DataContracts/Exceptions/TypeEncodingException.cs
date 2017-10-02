using System;

namespace Astral.Payloads.DataContracts
{
    public class TypeEncodingException : AstralException
    {
        public TypeEncodingException()
        {
        }

        public TypeEncodingException(string message) : base(message)
        {
        }

        public TypeEncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}