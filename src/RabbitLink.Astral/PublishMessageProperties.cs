using System;
using RabbitLink.Messaging;

namespace RabbitLink.Astral
{
    internal class PublishMessageProperties<TMessage>
    {
        public PublishMessageProperties(string sender, string replyTo, LinkDeliveryMode deliveryMode,
            Func<Lazy<TMessage>, TimeSpan> messageTtl, Func<Lazy<TMessage>, string> routingKey)
        {
            Sender = sender;
            ReplyTo = replyTo;
            DeliveryMode = deliveryMode;
            MessageTtl = messageTtl;
            RoutingKey = routingKey;
        }

        public string Sender { get; }
        public string ReplyTo { get; }
        public LinkDeliveryMode DeliveryMode { get; }
        public Func<Lazy<TMessage>, TimeSpan> MessageTtl { get; }
        public Func<Lazy<TMessage>, string> RoutingKey { get; }
    }
}