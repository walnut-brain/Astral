using System.Net.Mime;

namespace Astral.Payloads
{
    public class TextPayload : PayloadBase<string>
    {
        public TextPayload(string typeCode, ContentType contentType, string data) : base(typeCode, contentType, data)
        {
        }
    }
}