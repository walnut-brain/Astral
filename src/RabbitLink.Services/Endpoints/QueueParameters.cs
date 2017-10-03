using System;

namespace RabbitLink.Services
{
    public class QueueParameters
    {
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public TimeSpan? MessageTtl { get; set; }
        public TimeSpan? Expires { get; set; }
        public byte? MaxPriority { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxLengthBytes { get; set; }
        public string DeadLetterExchange { get; set; }
        public string DeadLetterRoutingKey { get; set; }
    }
}