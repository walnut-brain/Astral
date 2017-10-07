using System;
using System.Collections.Generic;

namespace RabbitLink.Services
{
    /// <summary>
    /// Queue parameters
    /// </summary>
    public class QueueParameters : BuilderBase
    {
        public QueueParameters()
        {
        }

        private QueueParameters(IReadOnlyDictionary<string, object> store) : base(store)
        {
        }

        /// <summary>
        /// get durable
        /// </summary>
        /// <returns>durable</returns>
        public bool Durable() => GetValue(nameof(Durable), true);
        /// <summary>
        /// set durable, default true
        /// </summary>
        /// <param name="value">durable value</param>
        /// <returns>parameters</returns>
        public QueueParameters Durable(bool value) => new QueueParameters(SetValue(nameof(Durable), value));

        /// <summary>
        /// get exclusive
        /// </summary>
        /// <returns>exclusive</returns>
        public bool Exclusive() => GetValue(nameof(Exclusive), false);
        /// <summary>
        /// set exclusive, default false
        /// </summary>
        /// <param name="value">exclusive value</param>
        /// <returns>parameters</returns>
        public QueueParameters Exclusive(bool value) => new QueueParameters(SetValue(nameof(Exclusive), value));

        /// <summary>
        /// get auto delete
        /// </summary>
        /// <returns>auto delete</returns>
        public bool AutoDelete() => GetValue(nameof(AutoDelete), false);
        /// <summary>
        /// set auto delete, default false
        /// </summary>
        /// <param name="value">auto delete value</param>
        /// <returns>parameters</returns>
        public QueueParameters AutoDelete(bool value) => new QueueParameters(SetValue(nameof(AutoDelete), value));

        /// <summary>
        /// get message ttl
        /// </summary>
        /// <returns>message ttl</returns>
        public TimeSpan? MessageTtl() => GetValue(nameof(MessageTtl), (TimeSpan?) null);
        /// <summary>
        /// set message ttl, default null
        /// </summary>
        /// <param name="value">message ttl</param>
        /// <returns>parameters</returns>
        public QueueParameters MessageTtl(TimeSpan? value) => new QueueParameters(SetValue(nameof(MessageTtl), value));
        
        /// <summary>
        /// get expires
        /// </summary>
        /// <returns>expires</returns>
        public TimeSpan? Expires() => GetValue(nameof(Expires), (TimeSpan?) null);
        /// <summary>
        /// set expires default null
        /// </summary>
        /// <param name="value">expires value</param>
        /// <returns>parameters</returns>
        public QueueParameters Expires(TimeSpan? value) => new QueueParameters(SetValue(nameof(Expires), value));

        /// <summary>
        /// get max priority
        /// </summary>
        /// <returns>max priority</returns>
        public byte? MaxPriority() => GetValue(nameof(MaxPriority), (byte?) null);
        /// <summary>
        /// set max priority, default null
        /// </summary>
        /// <param name="value">max priority value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxPriority(byte? value) => new QueueParameters(SetValue(nameof(MaxPriority), value));

        /// <summary>
        /// get max length
        /// </summary>
        /// <returns>max length</returns>
        public int? MaxLength() => GetValue(nameof(MaxLength), (int?) null);
        /// <summary>
        /// set max length, default null
        /// </summary>
        /// <param name="value">max length value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxLength(int? value) => new QueueParameters(SetValue(nameof(MaxLength), value));

        /// <summary>
        /// get max length bytes
        /// </summary>
        /// <returns>max length bytes</returns>
        public int? MaxLengthBytes() => GetValue(nameof(MaxLengthBytes), (int?) null);
        /// <summary>
        /// set max length bytes, default null
        /// </summary>
        /// <param name="value">max length bytes value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxLengthBytes(int? value) => new QueueParameters(SetValue(nameof(MaxLengthBytes), value));

        /// <summary>
        /// get dead letter exchange
        /// </summary>
        /// <returns>dead letter exchange</returns>
        public string DeadLetterExchange() => GetValue(nameof(DeadLetterExchange), (string) null);
        /// <summary>
        /// set dead letter exchange, default null
        /// </summary>
        /// <param name="value">dead letter exchange value</param>
        /// <returns>parameters</returns>
        public QueueParameters DeadLetterExchange(string value) => new QueueParameters(SetValue(nameof(DeadLetterExchange), value));

        /// <summary>
        /// get dead letter exchange routing key
        /// </summary>
        /// <returns>dead letter exchange routing key</returns>
        public string DeadLetterRoutingKey() => GetValue(nameof(DeadLetterRoutingKey), (string) null);
        /// <summary>
        /// set dead letter exchange routing key, default null
        /// </summary>
        /// <param name="value">dead letter exchange routing key value</param>
        /// <returns>parameters</returns>
        public QueueParameters DeadLetterRoutingKey(string value) => new QueueParameters(SetValue(nameof(DeadLetterRoutingKey), value));
    }
}