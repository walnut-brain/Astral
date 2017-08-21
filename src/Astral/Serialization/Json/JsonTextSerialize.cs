using System.Net.Mime;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
{
    public class JsonTextSerialize : ISerialize<string>
    {
        private readonly JsonSerializerSettings _settings;

        public JsonTextSerialize(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public Serialized<string> Serialize(string typeCode, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);
            return new Serialized<string>(typeCode, new ContentType("text/json"), json);
        }
    }
}