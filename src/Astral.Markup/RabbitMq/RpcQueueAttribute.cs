using System;

namespace Astral.Markup.RabbitMq
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RpcQueueAttribute : Attribute
    {
        public RpcQueueAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}