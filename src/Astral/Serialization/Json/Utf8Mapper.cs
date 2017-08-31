using System.Net.Mime;
using System.Text;
using Astral.Core;

namespace Astral.Serialization.Json
{
    public class Utf8Mapper : ISerializedMapper<string, byte[]>
    {
        public Payload<byte[]> Map(Payload<string> payload)
        {
            return new Payload<byte[]>(payload.TypeCode, new ContentType(payload.ContentType.MediaType)
            {
                CharSet = Encoding.UTF8.WebName
            }, Encoding.UTF8.GetBytes(payload.Data));
        }
    }
}