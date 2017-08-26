using System;
using Astral.Exceptions;
using LanguageExt;
using Newtonsoft.Json;

namespace Astral.Serialization.Json
{
    public class JsonTextDeserialize : IDeserialize<string>
    {
        private readonly bool _checkContextType;
        private readonly JsonSerializerSettings _settings;

        public JsonTextDeserialize(JsonSerializerSettings settings, bool checkContextType = false)
        {
            _settings = settings;
            _checkContextType = checkContextType;
        }

        public Try<object> Deserialize(Type type, Serialized<string> data)
        {
            if (!_checkContextType || data.ContentType?.IsJson() == true)
                return Prelude.Try(() => JsonConvert.DeserializeObject(data.Data, type, _settings))
                    .BiBind(Prelude.Try,
                        ex => Prelude.Try<object>(new DeserializationException(data.ContentType.ToString(),
                            data.TypeCode, type, ex)));
            return Prelude.Try<object>(new UnknownContentTypeException($"Unknown content type {data.ContentType}"));
        }
    }
}