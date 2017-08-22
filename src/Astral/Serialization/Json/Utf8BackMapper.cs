using System.Text;

namespace Astral.Serialization.Json
{
    public class Utf8BackMapper : ISerializedMapper<byte[], string>
    {
        public Serialized<string> Map(Serialized<byte[]> serialized)
        {
            var encode = Encoding.UTF8;
            if (serialized.ContentType?.CharSet != null)
            {
                try
                {
                    encode = Encoding.GetEncoding(serialized.ContentType?.CharSet);
                }
                catch 
                {
                    
                }
            }
            return new Serialized<string>(serialized.TypeCode, serialized.ContentType,
                encode.GetString(serialized.Data));
        }
    }
}