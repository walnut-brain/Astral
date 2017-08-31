using System;
using System.Text;
using Astral.Core;
using Astral.Exceptions;

namespace Astral.Serialization.Json
{
    public class Utf8BackMapper : ISerializedMapper<byte[], string>
    {
        public Payload<string> Map(Payload<byte[]> payload)
        {
            var encode = Encoding.UTF8;
            Exception exEnc = null;
            if (payload.ContentType?.CharSet != null)
                try
                {
                    encode = Encoding.GetEncoding(payload.ContentType?.CharSet);
                }
                catch (Exception ex)
                {
                    exEnc = ex;
                }
            try
            {
                return new Payload<string>(payload.TypeCode, payload.ContentType,
                    encode.GetString(payload.Data));
            }
            catch (Exception ex)
            {
                if (exEnc != null)
                    ex = new AggregateException(exEnc, ex);
                throw new EncodingErrorException(payload.ContentType?.CharSet, ex);
            }
        }
    }
}