using System.Collections.Generic;
using System.Net.Mime;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Internals;

namespace RabbitLink.Services
{
    internal abstract class Endpoint<TDescription> : BuilderBase
        where TDescription : EndpointDescription
    {
        protected TDescription Description { get; }
        protected ServiceLink Link { get; }

        protected Endpoint(ServiceLink link, TDescription description)
        {
            Description = description;
            Link = link;
        }

        protected Endpoint(ServiceLink link, TDescription description, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Description = description;
            Link = link;
        }
        
        public ContentType ContentType => Description.ContentType;
    }
}