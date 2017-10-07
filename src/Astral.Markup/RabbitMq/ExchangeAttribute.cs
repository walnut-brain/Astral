using System;

namespace Astral.Markup.RabbitMq
{
    /// <summary>
    /// Exchange option attribute.
    /// When used on service interface specify default exchange for events and request exchange for calls,
    /// without name specified used default exchange name in format '{Owner}.{Service}'. On the service level
    /// cannot be specified Fanout type.
    /// When used on event specify separate exchange, without name specified use name '{Owner}.{Service}.{Endpoint}'
    /// When used on call specify separate exchange, without name specified use name '{Owner}.{Service}.{Endpoint}' 
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class ExchangeAttribute : Attribute
    {
        /// <summary>
        /// Empty constructor. Usable on endpoint level to specify separate exchange
        /// </summary>
        public ExchangeAttribute()
        {
        }

        /// <summary>
        /// Named exchange declaration wit Direct type 
        /// </summary>
        /// <param name="name">exchange name</param>
        public ExchangeAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        /// <summary>
        /// Typed separate exchange declaration
        /// </summary>
        /// <param name="kind">exchange type</param>
        public ExchangeAttribute(ExchangeKind kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// Typed named separate exchange 
        /// </summary>
        /// <param name="name">exchange name</param>
        /// <param name="kind">exchange kind</param>
        public ExchangeAttribute(string name, ExchangeKind kind)
        {
            Kind = kind;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// exchange kind
        /// </summary>
        public ExchangeKind Kind { get; set; } = ExchangeKind.Direct;
        /// <summary>
        /// exchange name
        /// </summary>
        public string Name { get; set; } 
        /// <summary>
        /// durabiliyty, default true
        /// </summary>
        public bool Durable { get; set; } = true;
        /// <summary>
        /// auto delete, default false
        /// </summary>
        public bool AutoDelete { get; set; } = false;
        /// <summary>
        /// alternate exchange, default null - no alternate exchange
        /// </summary>
        public string Alternate { get; set; }
        /// <summary>
        /// delated exchange, default false 
        /// </summary>
        public bool Delayed { get; set; } = false;
    }
}