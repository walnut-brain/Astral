using System.Collections.Generic;
using System.Net.Mime;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;

namespace Astral.RabbitLink
{
    internal abstract class Endpoint<TSchema> : BuilderBase
        where TSchema : EndpointSchema<TSchema>
    {
        protected TSchema Description { get; }
        protected ServiceLink Link { get; }

        protected Endpoint(ServiceLink link, TSchema description)
        {
            Description = description;
            Link = link;
        }

        protected Endpoint(ServiceLink link, TSchema description, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }
        
        public ContentType ContentType => Description.ContentType();
    }
}