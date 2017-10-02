using System;

namespace RabbitLink.Astral.Markup
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