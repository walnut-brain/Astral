using System;

namespace Astral.Markup.RabbitMq
{
    /// <summary>
    /// Specify common call queue name for calls. Without specify used queue name '{Owner}.{Service}.{Endpoint}' 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RpcQueueAttribute : Attribute
    {
        public RpcQueueAttribute()
        {
        }

        public RpcQueueAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Durable { get; set; } = false;
        public bool AutoDelete { get; set; } = true;
    }
}