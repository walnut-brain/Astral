using System;

namespace Astral.Markup.RabbitMq
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class ResponseExchangeAttribute : Attribute
    {
        public ResponseExchangeAttribute(string name, ExchangeKind kind = ExchangeKind.Direct)
        {
            Kind = kind;
            Name = name;
        }

        public ExchangeKind Kind { get; }
        public string Name { get; }
    }
}