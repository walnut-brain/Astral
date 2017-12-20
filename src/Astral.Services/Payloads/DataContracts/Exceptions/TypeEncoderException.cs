using System;

namespace Astral.Payloads.DataContracts.Exceptions
{
    public class TypeEncoderException : TypeEncodingException
    {
        public TypeEncoderException(Type type)
            : this(type, $"Cannot determine contract name of {type}")
        {
        }

        public TypeEncoderException(Type type, string message) : base(message)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}