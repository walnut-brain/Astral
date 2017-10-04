using System;

namespace RabbitLink.Services.Astral
{
    public class SchemaMarkupException : Exception
    {
        public SchemaMarkupException()
        {
        }

        public SchemaMarkupException(string message) : base(message)
        {
        }

        public SchemaMarkupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}