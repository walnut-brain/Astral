using System.Collections.Generic;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public class RequestQueueSchema : IRequestQueueSchema
    {
        public RequestQueueSchema(string name, bool durable = false, bool autoDelete = true)
        {
            Name = name;
            Durable = durable;
            AutoDelete = autoDelete;
        }

        public string Name { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
    }
}