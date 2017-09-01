using System.Net.Mime;

namespace Astral.Payloads
{
    public class RawPayload : PayloadBase<byte[]>
    {
        public RawPayload(string typeCode, ContentType contentType, byte[] data) : base(typeCode, contentType, data)
        {
        }
    }
}