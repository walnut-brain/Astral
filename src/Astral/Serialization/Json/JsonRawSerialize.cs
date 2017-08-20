using System.Text;
using Newtonsoft.Json;

namespace WalnutBrain.Bus.Serialization.Json
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
            return new Serialized<byte[]>(typeCode, "text/json;encoding=utf8", bytes);
        }
    }
}