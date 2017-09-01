using System.Net.Mime;
using System.Text;
using Astral.Payloads;

namespace Astral.Serialization.Json
{
    public class Utf8Mapper : ISerializedMapper<string, byte[]>
    {
        public PayloadBase<byte[]> Map(PayloadBase<string> payload)
        {
            return new PayloadBase<byte[]>(payload.TypeCode, new ContentType(payload.ContentType.MediaType)
            {
                CharSet = Encoding.UTF8.WebName
            }, Encoding.UTF8.GetBytes(payload.Data));
        }
    }
}