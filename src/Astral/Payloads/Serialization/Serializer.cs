using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Payloads.Serialization
{
    public class Serializer<TFormat> 
    {
        public Serializer(SerializeProvider<TFormat> serialize, DeserializeProvider<TFormat> deserialize)
        {
            Serialize = serialize;
            Deserialize = deserialize;
        }

        public SerializeProvider<TFormat> Serialize { get; }
        public DeserializeProvider<TFormat> Deserialize { get; }
        
    }

    public static class Serializer
    {
        public static Serializer<byte[]> JsonRaw =
            new Serializer<byte[]>(Serialization.JsonRawSerializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        
        public static Serializer<string> JsonText =
            new Serializer<string>(Serialization.JsonTextSerializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), Serialization.JsonTextDeserializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
    }
}