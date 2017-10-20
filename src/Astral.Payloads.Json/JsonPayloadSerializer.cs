using System;
using System.Linq;
using System.Net.Mime;
using Astral.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Payloads.Json
{
    public class JsonPayloadSerializer : ITextPayloadSerializer
    {
        private readonly JsonSerializerSettings _settings;


        public JsonPayloadSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        private static bool IsJson(ContentType contentType)
        {
            var types = new[] {"text/json", "application/json"};

            return types.Any(p =>
                string.Compare(contentType.MediaType, p, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public bool SupportContentType(string contentType)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));
            return IsJson(new ContentType(contentType));
        }

        public string Serialize<T>(T value, ITypeSchema schema)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        public T Deserialize<T>(string body, ITypeSchema schema)
        {
            var type = typeof(T);
            return JsonConvert.DeserializeObject<T>(body, _settings);
        }
    }
}