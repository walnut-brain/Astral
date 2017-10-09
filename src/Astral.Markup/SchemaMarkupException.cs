using System;

namespace Astral.Markup
{
    /// <summary>
    /// Schema astral markup using exception
    /// </summary>
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