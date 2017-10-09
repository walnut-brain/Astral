using System;

namespace Astral.Markup.RabbitMq
{
    /// <summary>
    /// Response exchange option attribute.
    /// Cannot be Fanout type
    /// With name specified as empty string default rabbitmq exchange used for responses
    /// When used on service interface specify default response exchange for calls, when not specified use exchange setup.
    /// Without name specified used default exchange name in format '{Owner}.{Service}.responses'. 
    /// When used on call specify separate exchange, without name specified use name '{Owner}.{Service}.{Endpoint}.reponses'
    /// When not specified on endpoint level service response exchange used
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class ResponseExchangeAttribute : Attribute
    {
        private ExchangeKind _kind = ExchangeKind.Direct;

        /// <summary>
        /// Empty constructor. Usable on endpoint level to specify separate exchange
        /// </summary>
        public ResponseExchangeAttribute()
        {
        }

        /// <summary>
        /// Named exchange declaration wit Direct type 
        /// </summary>
        /// <param name="name">exchange name</param>
        public ResponseExchangeAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        /// <summary>
        /// Typed separate exchange declaration
        /// </summary>
        /// <param name="kind">exchange type</param>
        public ResponseExchangeAttribute(ExchangeKind kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// Typed named separate exchange 
        /// </summary>
        /// <param name="name">exchange name</param>
        /// <param name="kind">exchange kind</param>
        public ResponseExchangeAttribute(string name, ExchangeKind kind)
        {
            Kind = kind;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// exchange kind
        /// </summary>
        public ExchangeKind Kind
        {
            get { return _kind; }
            set
            {
                if(value == ExchangeKind.Fanout)
                    throw new ArgumentOutOfRangeException("Response exchange cannot be fanout");
                _kind = value;
            }
        }

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