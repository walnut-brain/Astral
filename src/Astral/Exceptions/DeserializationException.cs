using System;

namespace Astral.Exceptions
{
    public class DeserializationException : PermanentException
    {
        public DeserializationException(string contentType, string typeCode, Type toType) 
            : base($"Cannot deserialize {contentType} {typeCode} to {toType}")
        {
        }

        public DeserializationException(string contentType, string typeCode, Type toType, Exception innerException) :
            base($"Cannot deserialize {contentType} {typeCode} to {toType}", innerException)
        {
        }
    }
}