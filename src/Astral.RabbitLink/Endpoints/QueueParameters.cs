using System;
using System.Collections.Generic;

namespace Astral.RabbitLink
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
        public bool Durable() => GetParameter(nameof(Durable), true);
        /// <summary>
        /// set durable, default true
        /// </summary>
        /// <param name="value">durable value</param>
        /// <returns>parameters</returns>
        public QueueParameters Durable(bool value) => new QueueParameters(SetParameter(nameof(Durable), value));

        /// <summary>
        /// get exclusive
        /// </summary>
        /// <returns>exclusive</returns>
        public bool Exclusive() => GetParameter(nameof(Exclusive), false);
        /// <summary>
        /// set exclusive, default false
        /// </summary>
        /// <param name="value">exclusive value</param>
        /// <returns>parameters</returns>
        public QueueParameters Exclusive(bool value) => new QueueParameters(SetParameter(nameof(Exclusive), value));

        /// <summary>
        /// get auto delete
        /// </summary>
        /// <returns>auto delete</returns>
        public bool AutoDelete() => GetParameter(nameof(AutoDelete), false);
        /// <summary>
        /// set auto delete, default false
        /// </summary>
        /// <param name="value">auto delete value</param>
        /// <returns>parameters</returns>
        public QueueParameters AutoDelete(bool value) => new QueueParameters(SetParameter(nameof(AutoDelete), value));

        /// <summary>
        /// get message ttl
        /// </summary>
        /// <returns>message ttl</returns>
        public TimeSpan? MessageTtl() => GetParameter(nameof(MessageTtl), (TimeSpan?) null);
        /// <summary>
        /// set message ttl, default null
        /// </summary>
        /// <param name="value">message ttl</param>
        /// <returns>parameters</returns>
        public QueueParameters MessageTtl(TimeSpan? value) => new QueueParameters(SetParameter(nameof(MessageTtl), value));
        
        /// <summary>
        /// get expires
        /// </summary>
        /// <returns>expires</returns>
        public TimeSpan? Expires() => GetParameter(nameof(Expires), (TimeSpan?) null);
        /// <summary>
        /// set expires default null
        /// </summary>
        /// <param name="value">expires value</param>
        /// <returns>parameters</returns>
        public QueueParameters Expires(TimeSpan? value) => new QueueParameters(SetParameter(nameof(Expires), value));

        /// <summary>
        /// get max priority
        /// </summary>
        /// <returns>max priority</returns>
        public byte? MaxPriority() => GetParameter(nameof(MaxPriority), (byte?) null);
        /// <summary>
        /// set max priority, default null
        /// </summary>
        /// <param name="value">max priority value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxPriority(byte? value) => new QueueParameters(SetParameter(nameof(MaxPriority), value));

        /// <summary>
        /// get max length
        /// </summary>
        /// <returns>max length</returns>
        public int? MaxLength() => GetParameter(nameof(MaxLength), (int?) null);
        /// <summary>
        /// set max length, default null
        /// </summary>
        /// <param name="value">max length value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxLength(int? value) => new QueueParameters(SetParameter(nameof(MaxLength), value));

        /// <summary>
        /// get max length bytes
        /// </summary>
        /// <returns>max length bytes</returns>
        public int? MaxLengthBytes() => GetParameter(nameof(MaxLengthBytes), (int?) null);
        /// <summary>
        /// set max length bytes, default null
        /// </summary>
        /// <param name="value">max length bytes value</param>
        /// <returns>parameters</returns>
        public QueueParameters MaxLengthBytes(int? value) => new QueueParameters(SetParameter(nameof(MaxLengthBytes), value));

        /// <summary>
        /// get dead letter exchange
        /// </summary>
        /// <returns>dead letter exchange</returns>
        public string DeadLetterExchange() => GetParameter(nameof(DeadLetterExchange), (string) null);
        /// <summary>
        /// set dead letter exchange, default null
        /// </summary>
        /// <param name="value">dead letter exchange value</param>
        /// <returns>parameters</returns>
        public QueueParameters DeadLetterExchange(string value) => new QueueParameters(SetParameter(nameof(DeadLetterExchange), value));

        /// <summary>
        /// get dead letter exchange routing key
        /// </summary>
        /// <returns>dead letter exchange routing key</returns>
        public string DeadLetterRoutingKey() => GetParameter(nameof(DeadLetterRoutingKey), (string) null);
        /// <summary>
        /// set dead letter exchange routing key, default null
        /// </summary>
        /// <param name="value">dead letter exchange routing key value</param>
        /// <returns>parameters</returns>
        public QueueParameters DeadLetterRoutingKey(string value) => new QueueParameters(SetParameter(nameof(DeadLetterRoutingKey), value));
    }
}