using System.Net.Mime;
using Astral.Payloads;
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

        public PayloadBase<string> Serialize(string typeCode, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);
            return new PayloadBase<string>(typeCode, new ContentType("text/json"), json);
        }
    }
}