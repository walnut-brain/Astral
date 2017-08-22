using System;

namespace Astral.Exceptions
{
    public class RequeuException : Exception
    {
        public RequeuException()
        {
        }

        public RequeuException(string message) : base(message)
        {
        }
    }
}