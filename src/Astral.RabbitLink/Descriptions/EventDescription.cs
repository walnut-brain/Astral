using System;
using System.Net.Mime;

namespace Astral.RabbitLink.Descriptions
{
    /// <summary>
    /// Event description
    /// </summary>
    public class EventDescription : EndpointDescription
    {
        /// <summary>
        /// constructor with routing key
        /// </summary>
        /// <param name="service">service description</param>
        /// <param name="name">event name</param>
        /// <param name="type">event contract type</param>
        /// <param name="contentType">content type</param>
        /// <param name="exchange">exchange description</param>
        /// <param name="routingKey">routing key</param>
        public EventDescription(ServiceDescription service, string name, Type type, ContentType contentType, ExchangeDescription exchange, string routingKey)
            : base(service, name, contentType, routingKey)
        {
            if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// constructor with routing key factory
        /// </summary>
        /// <param name="service">service description</param>
        /// <param name="name">event name</param>
        /// <param name="type">event contract type</param>
        /// <param name="contentType">content type</param>
        /// <param name="exchange">exchange description</param>
        /// <param name="routingKeyExtractor">routing key from event factory</param>
        public EventDescription(ServiceDescription service, string name, Type type, ContentType contentType, ExchangeDescription exchange, Func<object, string> routingKeyExtractor) : base(service, name,
            contentType, null)
        {
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            RoutingKeyExtractor = routingKeyExtractor ?? throw new ArgumentNullException(nameof(routingKeyExtractor));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// exchange description
        /// </summary>
        public ExchangeDescription Exchange { get; }
        /// <summary>
        /// routing key from event extractor
        /// </summary>
        public Func<object, string> RoutingKeyExtractor { get; }
        /// <summary>
        /// event contract type
        /// </summary>
        public Type Type { get; }
    }
}