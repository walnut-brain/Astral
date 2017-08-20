using Newtonsoft.Json;

namespace WalnutBrain.Bus.Serialization.Json
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
            return new Serialized<string>(typeCode, "text/json", json);
        }
    }
}