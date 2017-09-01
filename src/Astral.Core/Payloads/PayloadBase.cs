using System.Net.Mime;

namespace Astral.Payloads
{
    public class PayloadBase<TFormat>
    {
        public PayloadBase(string typeCode, ContentType contentType, TFormat data)
        {
            TypeCode = typeCode;
            ContentType = contentType;
            Data = data;
        }

        public string TypeCode { get; }
        public ContentType ContentType { get; }
        public TFormat Data { get; }
    }
}