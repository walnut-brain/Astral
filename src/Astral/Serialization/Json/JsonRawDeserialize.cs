using System.Text;
using LanguageExt;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
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
            var encoding = Encoding.UTF8;
            if(data.ContentType.CharSet != null)
                try
                {
                    encoding = Encoding.GetEncoding(data.ContentType.CharSet);
                }
                catch 
                {
                    
                }

            return Prelude.Try(() => encoding.GetString(data.Data))
                .Bind(p => Prelude.Try(() => JsonConvert.DeserializeObject<T>(p, _settings)))();
        }
    }
}