using System.Net.Mime;

namespace RabbitLink.Services.Descriptions
{
    public class CallDescription
    {
        public ExchangeDescription RequestExchange { get; set; }
        public ExchangeDescription ResponseExchange { get; set; }
        public ContentType ContentType { get; set; }
    }
}