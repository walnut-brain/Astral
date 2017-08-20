using System.Text;

namespace WalnutBrain.Bus.Serialization.Json
{
    public class Utf8Mapper : ISerializedMapper<string, byte[]>
    {
        public Serialized<byte[]> Map(Serialized<string> serialized)
        {
            return new Serialized<byte[]>(serialized.TypeCode, serialized.ContentType + ";encoding=Utf8",
                Encoding.UTF8.GetBytes(serialized.Data));
        }
    }
}