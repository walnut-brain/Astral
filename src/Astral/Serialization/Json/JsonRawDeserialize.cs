using System.Text;
using LanguageExt;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace WalnutBrain.Bus.Serialization.Json
{
    public class JsonRawDeserialize : IDeserialize<byte[]>
    {
        private readonly JsonSerializerSettings _settings;

        public JsonRawDeserialize(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public Result<T> Deserialize<T>(Serialized<byte[]> data)
        {
            return Try(() => Encoding.UTF8.GetString(data.Data))
                .Bind(p => Try(() => JsonConvert.DeserializeObject<T>(p, _settings)))();
        }
    }
}