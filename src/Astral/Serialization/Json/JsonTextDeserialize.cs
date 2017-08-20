using LanguageExt;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace WalnutBrain.Bus.Serialization.Json
{
    public class JsonTextDeserialize : IDeserialize<string>
    {
        private readonly JsonSerializerSettings _settings;

        public JsonTextDeserialize(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public Result<T> Deserialize<T>(Serialized<string> data)
        {
            return Try(() => JsonConvert.DeserializeObject<T>(data.Data, _settings))();
        }
    }
}