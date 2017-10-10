using System.Collections.Generic;
using System.Net.Mime;
using Astral.Logging;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;

namespace Astral.RabbitLink
{
    internal abstract class Endpoint<TSchema> : BuilderBase
        where TSchema : EndpointSchema<TSchema>
    {
        public TSchema Schema { get; }
        protected ServiceLink Link { get; }
        protected ILog Log { get; }

        protected Endpoint(ServiceLink link, TSchema schema)
        {
            Schema = schema;
            Link = link;
            Log = link.LogFactory.CreateLog(GetType()).With("service", schema.Service.Name)
                .With("endpoint", schema.Name);
        }

        protected Endpoint(ServiceLink link, TSchema schema, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Schema = schema;
            Link = link;
            Log = link.LogFactory.CreateLog(GetType()).With("service", schema.Service.Name)
                .With("endpoint", schema.Name);
        }
        
        public ContentType ContentType => Schema.ContentType();
    }
}