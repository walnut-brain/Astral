using System;

namespace Astral.Exceptions
{
    public class EncodingErrorException : EncodingException
    {
        public EncodingErrorException(string encoding) : base($"Can encode from {encoding}")
        {
        }

        public EncodingErrorException(string encoding, Exception innerException) : base($"Can encode from {encoding}", innerException)
        {
        }
    }
}