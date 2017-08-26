using System;
using System.Text;
using Astral.Exceptions;

namespace Astral.Serialization.Json
{
    public class Utf8BackMapper : ISerializedMapper<byte[], string>
    {
        public Serialized<string> Map(Serialized<byte[]> serialized)
        {
            var encode = Encoding.UTF8;
            Exception exEnc = null;
            if (serialized.ContentType?.CharSet != null)
                try
                {
                    encode = Encoding.GetEncoding(serialized.ContentType?.CharSet);
                }
                catch (Exception ex)
                {
                    exEnc = ex;
                }
            try
            {
                return new Serialized<string>(serialized.TypeCode, serialized.ContentType,
                    encode.GetString(serialized.Data));
            }
            catch (Exception ex)
            {
                if (exEnc != null)
                    ex = new AggregateException(exEnc, ex);
                throw new EncodingErrorException(serialized.ContentType?.CharSet, ex);
            }
        }
    }
}