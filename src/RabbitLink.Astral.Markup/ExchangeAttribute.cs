using System;


namespace RabbitLink.Astral.Markup
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class ExchangeAttribute : Attribute
    {
        public ExchangeAttribute(string name, ExchangeKind kind = ExchangeKind.Direct)
        {
            Kind = kind;
            Name = name;
        }

        public ExchangeKind Kind { get; }
        public string Name { get; }
    }
}