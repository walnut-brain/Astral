using System;

namespace Astral.Markup.RabbitMq
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RoutingKeyAttribute : Attribute
    {
        public RoutingKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}