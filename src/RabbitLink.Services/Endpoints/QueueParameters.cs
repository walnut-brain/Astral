using System;
using System.Collections.Generic;

namespace RabbitLink.Services
{
    public class QueueParameters : BuilderBase
    {
        public QueueParameters()
        {
        }

        private QueueParameters(IReadOnlyDictionary<string, object> store) : base(store)
        {
        }

        public bool Durable() => GetValue(nameof(Durable), true);
        public QueueParameters Durable(bool value) => new QueueParameters(SetValue(nameof(Durable), value));

        public bool Exclusive() => GetValue(nameof(Exclusive), false);
        public QueueParameters Exclusive(bool value) => new QueueParameters(SetValue(nameof(Exclusive), value));

        public bool AutoDelete() => GetValue(nameof(AutoDelete), false);
        public QueueParameters AutoDelete(bool value) => new QueueParameters(SetValue(nameof(AutoDelete), value));

        public TimeSpan? MessageTtl() => GetValue(nameof(MessageTtl), (TimeSpan?) null);
        public QueueParameters MessageTtl(TimeSpan? value) => new QueueParameters(SetValue(nameof(MessageTtl), value));
        
        public TimeSpan? Expires() => GetValue(nameof(Expires), (TimeSpan?) null);
        public QueueParameters Expires(TimeSpan? value) => new QueueParameters(SetValue(nameof(Expires), value));

        public byte? MaxPriority() => GetValue(nameof(MaxPriority), (byte?) null);
        public QueueParameters MaxPriority(byte? value) => new QueueParameters(SetValue(nameof(MaxPriority), value));

        public int? MaxLength() => GetValue(nameof(MaxLength), (int?) null);
        public QueueParameters MaxLength(int? value) => new QueueParameters(SetValue(nameof(MaxLength), value));

        public int? MaxLengthBytes() => GetValue(nameof(MaxLengthBytes), (int?) null);
        public QueueParameters MaxLengthBytes(int? value) => new QueueParameters(SetValue(nameof(MaxLengthBytes), value));

        public string DeadLetterExchange() => GetValue(nameof(DeadLetterExchange), (string) null);
        public QueueParameters DeadLetterExchange(string value) => new QueueParameters(SetValue(nameof(DeadLetterExchange), value));

        public string DeadLetterRoutingKey() => GetValue(nameof(DeadLetterRoutingKey), (string) null);
        public QueueParameters DeadLetterRoutingKey(string value) => new QueueParameters(SetValue(nameof(DeadLetterRoutingKey), value));
    }
}