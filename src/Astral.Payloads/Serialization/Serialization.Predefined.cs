using System.Net.Mime;
using System.Text;
using Astral.Fun.Monads;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Payloads.Serialization
{
    public static partial class Serialization
    {
        public static SerializeProvider<string> JsonTextSerializeProvider(JsonSerializerSettings settings)
        {
            return SerializeProvider(Extensions.IsJson,
                o => Result.Try(() => JsonConvert.SerializeObject(o, settings)));
        }

        public static SerializeProvider<byte[]> JsonRawSerializeProvider(JsonSerializerSettings settings)
        {
            return SerializeProvider(Extensions.IsJson,
                (ct, o) => Result.Try(() => JsonConvert.SerializeObject(o, settings))
                    .Map(p => Encode(ct, p)));
        }

        public static DeserializeProvider<string> JsonTextDeserializeProvider(JsonSerializerSettings settings)
        {
            return DeserializeProvider<string>(Extensions.IsJson,
                (type, data) => Result.Try(() => JsonConvert.DeserializeObject(data, type, settings)));
        }

        public static DeserializeProvider<byte[]> JsonRawDeserializeProvider(JsonSerializerSettings settings)
        {
            return DeserializeProvider(Extensions.IsJson,
                ct => Deserialize<byte[]>((type, data) => Result.Try(() => Decode(ct, data)).Map(txt =>
                    JsonConvert.DeserializeObject(txt, type, settings))));
        }


        private static byte[] Encode(ContentType ct, string text)
        {
            var encodingName = ct.CharSet ?? Encoding.UTF8.WebName;
            var encoding = Encoding.GetEncoding(encodingName);
            return encoding.GetBytes(text);
        }

        private static string Decode(Option<ContentType> ct, byte[] data)
        {
            var encodingName = ct.Bind(p => p.CharSet.ToOption()).IfNone(() => Encoding.UTF8.WebName);
            var encoding = Encoding.GetEncoding(encodingName);
            return encoding.GetString(data);
        }
        
        public static Serialization<byte[]> JsonRaw =
            new Serialization<byte[]>(JsonRawSerializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), JsonRawDeserializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        
        public static Serialization<string> JsonText =
            new Serialization<string>(JsonTextSerializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), JsonTextDeserializeProvider(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
    }
}