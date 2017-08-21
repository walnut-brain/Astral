using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
{
    public class JsonRawSerialize : ISerialize<byte[]>
    {
        private readonly JsonSerializerSettings _settings;

        public JsonRawSerialize(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public Serialized<byte[]> Serialize(string typeCode, object obj)
        {
            var text = JsonConvert.SerializeObject(obj, _settings);
            var bytes = Encoding.UTF8.GetBytes(text);
            return new Serialized<byte[]>(typeCode, new ContentType("text/json")
            {
                CharSet = Encoding.UTF8.WebName
            }, bytes);
        }
    }
}