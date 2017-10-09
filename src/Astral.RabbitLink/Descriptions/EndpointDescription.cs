using System;
using System.Net.Mime;

namespace Astral.RabbitLink.Descriptions
{
    /// <summary>
    /// Endpoint description
    /// </summary>
    public abstract class EndpointDescription
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">service description</param>
        /// <param name="name">endpoint name</param>
        /// <param name="contentType">content type</param>
        /// <param name="routingKey">routing key, can be null</param>
        protected EndpointDescription(ServiceDescription service, string name, ContentType contentType, string routingKey)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            RoutingKey = routingKey;
            Service = service ?? throw new ArgumentNullException(nameof(service));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// content type
        /// </summary>
        public ContentType ContentType { get;  }
        /// <summary>
        /// routing key
        /// </summary>
        public string RoutingKey { get; }
        /// <summary>
        /// service description
        /// </summary>
        public ServiceDescription Service { get; }
        /// <summary>
        /// endpoint name
        /// </summary>
        public string Name { get; }
    }
}