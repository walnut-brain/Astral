using System.Net.Mime;

namespace RabbitLink.Services.Descriptions
{
    public class CallDescription
    {
        public CallDescription(ServiceDescription service, ExchangeDescription requestExchange,
            ExchangeDescription responseExchange, ContentType contentType, string rpcQueueName, string routingKey, string name)
        {
            RequestExchange = requestExchange;
            ResponseExchange = responseExchange;
            ContentType = contentType;
            RpcQueueName = rpcQueueName;
            RoutingKey = routingKey;
            Name = name;
            Service = service;
        }

        public ExchangeDescription RequestExchange { get;  }
        public ExchangeDescription ResponseExchange { get;  }
        public ContentType ContentType { get;  }
        public string RpcQueueName { get; }
        public string RoutingKey { get; }
        public ServiceDescription Service { get; }
        public string Name { get; }
        
    }
}