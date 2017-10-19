using Astral.Schema.Data;
using Astral.Schema.Green;
using Astral.Schema.RabbitMq;

namespace Astral.Schema.Json
{
    public static class Extensions
    {
        public static JsonSchemaStore Json(this ServiceSchema schema)
            => new JsonSchemaStore(schema);
        
    }

    internal class RefTypeSchemaGreen : TypeSchemaGreen
    {
        public RefTypeSchemaGreen() : base(Option.None, false)
        {
        }
    }
}