using System;
using System.Net.Mime;
using System.Text;


namespace Astral.Payloads
{
    public class TextPayload
    {
        public TextPayload(string contentType, string typeHint, string body)
        {
            ContentType = contentType;
            TypeHint = typeHint;
            Body = body;
        }

        public TextPayload(RawPayload rawPayload)
        {
            var encoding = Encoding.UTF8;
            ContentType = null;
            if (rawPayload.ContentType != null)
            {
                var contentType = new ContentType(rawPayload.ContentType);
                if (contentType.CharSet != null)
                {
                    encoding = Encoding.GetEncoding(contentType.CharSet);
                    contentType.CharSet = null;
                }
                ContentType = contentType.ToString();
            }
            TypeHint = rawPayload.TypeHint;
            Body = encoding.GetString(rawPayload.Body);
        }

        public string ContentType { get; }
        public string TypeHint { get; }
        public string Body { get; }
    }


    
}