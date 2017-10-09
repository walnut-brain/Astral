using System;
using System.Net.Mime;

namespace Astral.RabbitLink.Descriptions
{
    /// <summary>
    /// Call description
    /// </summary>
    public class CallDescription : EndpointDescription
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="service">service description</param>
        /// <param name="name">call name</param>
        /// <param name="requestType">request contract type</param>
        /// <param name="responseType">response contract type</param>
        /// <param name="contentType">content type</param>
        /// <param name="requestExchange">request exchange description</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="responseExchange">response exchange description</param>
        /// <param name="queueName">rpc queue name</param>
        /// <param name="queueDurable">rpc queue durability</param>
        /// <param name="queueAutoDelete">rpc queue autodelete</param>
        public CallDescription(ServiceDescription service, string name, Type requestType, Type responseType,
            ContentType contentType, ExchangeDescription requestExchange, string routingKey,
            ExchangeDescription responseExchange, string queueName, bool queueDurable, bool queueAutoDelete) : base(
            service, name, contentType, routingKey)
        {
            if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));
            QueueDurable = queueDurable;
            QueueAutoDelete = queueAutoDelete;
            RequestType = requestType;
            ResponseType = responseType;
            RequestExchange = requestExchange ?? throw new ArgumentNullException(nameof(requestExchange));
            ResponseExchange = responseExchange ?? throw new ArgumentNullException(nameof(responseExchange));
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        /// <summary>
        /// request exchange description
        /// </summary>
        public ExchangeDescription RequestExchange { get;  }
        /// <summary>
        /// response exchange description
        /// </summary>
        public ExchangeDescription ResponseExchange { get;  }
        /// <summary>
        /// name of rpc queue
        /// </summary>
        public string QueueName { get; }
        /// <summary>
        /// rpc queue durability
        /// </summary>
        public bool QueueDurable { get; }
        /// <summary>
        /// rpc queue autodelete
        /// </summary>
        public bool QueueAutoDelete { get; }
        /// <summary>
        /// request constract type
        /// </summary>
        public Type RequestType { get; }
        /// <summary>
        /// response contract type
        /// </summary>
        public Type ResponseType { get; }
    }
}