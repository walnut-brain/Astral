using LanguageExt;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
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
            return Prelude.Try(() => JsonConvert.DeserializeObject<T>(data.Data, _settings))();
        }
    }
}