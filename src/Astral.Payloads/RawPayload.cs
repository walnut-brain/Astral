using System.Net.Mime;
using System.Text;

namespace Astral.Payloads
{
    public class RawPayload
    {
        public RawPayload(string contentType, string typeHint, byte[] body)
        {
            ContentType = contentType;
            TypeHint = typeHint;
            Body = body;
        }

        public RawPayload(TextPayload textPayload, string charset = "utf-8")
        {
            var encoding = Encoding.GetEncoding(charset);
            TypeHint = textPayload.TypeHint;
            ContentType =
                textPayload.ContentType == null
                    ? null
                    : new ContentType(textPayload.ContentType) {CharSet = encoding.WebName}.ToString();
            Body = encoding.GetBytes(textPayload.Body);
        }

        public string ContentType { get; }
        public string TypeHint { get; }
        public byte[] Body { get; }
    }
}