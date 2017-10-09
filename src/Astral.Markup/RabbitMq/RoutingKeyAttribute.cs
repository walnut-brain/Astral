using System;

namespace Astral.Markup.RabbitMq
{
    /// <summary>
    /// Specify routing key for endpoint. Without routing key specification use {Endpoint} as routing key.
    /// Ignored when specified on fanout exchange 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RoutingKeyAttribute : Attribute
    {
        public RoutingKeyAttribute(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            Key = key;
        }

        public string Key { get; }
    }
}