using System.Text;

namespace WalnutBrain.Bus.Serialization.Json
{
    public class Utf8BackMapper : ISerializedMapper<byte[], string>
    {
        public Serialized<string> Map(Serialized<byte[]> serialized)
        {
            return new Serialized<string>(serialized.TypeCode, serialized.ContentType,
                Encoding.UTF8.GetString(serialized.Data));
        }
    }
}