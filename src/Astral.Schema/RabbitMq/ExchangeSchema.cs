using Astral.Markup.RabbitMq;

namespace Astral.Schema.RabbitMq
{
    /// <summary>
    /// exchange description
    /// </summary>
    public class ExchangeSchema
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">exchange name</param>
        /// <param name="type">exchange type</param>
        /// <param name="durable">exchange durability</param>
        /// <param name="autoDelete">exchange autodelete</param>
        /// <param name="delayed">exchange dealyed</param>
        /// <param name="alternate">exchange alternate</param>
        public ExchangeSchema(string name, ExchangeKind type = ExchangeKind.Direct, bool durable = true,
            bool autoDelete = false, bool delayed = false, string alternate = null
            )
        {
            Name = name;
            Durable = durable;
            AutoDelete = autoDelete;
            Delayed = delayed;
            Alternate = alternate;
            Type = type;
        }

        /// <summary>
        /// exchange name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// exchange type
        /// </summary>
        public ExchangeKind Type { get; } //= LinkExchangeType.Direct;
        /// <summary>
        /// exchange durability
        /// </summary>
        public bool Durable { get; } //= true;
        /// <summary>
        /// exchange auto delete
        /// </summary>
        public bool AutoDelete { get; } //= false;
        /// <summary>
        /// alternate exchange
        /// </summary>
        public string Alternate { get; }
        /// <summary>
        /// exchange delayed
        /// </summary>
        public bool Delayed { get; } //= false;
    }
}