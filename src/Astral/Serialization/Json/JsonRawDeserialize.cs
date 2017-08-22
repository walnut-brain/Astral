using System;
using System.Text;
using Astral.Exceptions;
using LanguageExt;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
{
    public class JsonRawDeserialize : IDeserialize<byte[]>
    {
        private readonly JsonSerializerSettings _settings;
        private readonly bool _checkContentType;

        public JsonRawDeserialize(JsonSerializerSettings settings, bool checkContentType = false)
        {
            _settings = settings;
            _checkContentType = checkContentType;
        }


        public Try<object> Deserialize(Type type, Serialized<byte[]> data)
        {
            if (!_checkContentType || data.ContentType?.IsJson() == true)
            {
                return Prelude.Try(() => new Utf8BackMapper().Map(data))
                    .Bind(p => Prelude.Try(() => JsonConvert.DeserializeObject(p.Data, type,_settings)));
            }
            return Prelude.Try<object>(new UnknownContentTypeException($"Unknown content type {data.ContentType}"));
        }
    }
}