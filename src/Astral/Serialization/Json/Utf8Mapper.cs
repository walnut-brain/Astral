using System.Net.Mime;
using System.Text;

namespace Astral.Serialization.Json
{
    public class Utf8Mapper : ISerializedMapper<string, byte[]>
    {
        public Serialized<byte[]> Map(Serialized<string> serialized)
        {
            return new Serialized<byte[]>(serialized.TypeCode, new ContentType(serialized.ContentType.MediaType)
            {
                CharSet = Encoding.UTF8.WebName
            }, Encoding.UTF8.GetBytes(serialized.Data));
        }
    }
}