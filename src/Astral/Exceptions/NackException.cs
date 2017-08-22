using System;

namespace Astral.Exceptions
{
    public class NackException : Exception
    {
        public NackException()
        {
        }

        public NackException(string message) : base(message)
        {
        }
    }
}