using System;

namespace Astral.Schema
{
    public class SchemaException : Exception
    {
        public SchemaException()
        {
        }

        public SchemaException(string message) : base(message)
        {
        }

        public SchemaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}