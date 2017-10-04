using System;

namespace Astral
{
    public class AstralException : Exception
    {
        public AstralException()
        {
        }

        public AstralException(string message) : base(message)
        {
        }

        public AstralException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}