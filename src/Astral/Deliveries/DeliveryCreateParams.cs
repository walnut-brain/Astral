using System;

namespace Astral.Deliveries
{
    public class DeliveryCreateParams<T> 
    {
        public DeliveryCreateParams(Guid deliveryId, string target, string sender, string service, string endpoint, string key, 
            DeliveryReplyTo replyTo, string replayOn, T message, bool isReply)
        {
            DeliveryId = deliveryId;
            Target = target;
            Sender = sender;
            Service = service;
            Endpoint = endpoint;
            Key = key;
            ReplyTo = replyTo;
            ReplayOn = replayOn;
            Message = message;
            IsReply = isReply;
        }

        public Guid DeliveryId { get; }
        public string Target { get; }
        public string Sender { get; }
        public string Service { get; }
        public string Endpoint { get; }
        public string Key { get; }
        public DeliveryReplyTo ReplyTo { get; }
        public string ReplayOn { get; }
        public bool IsReply { get; }
        public T Message { get; }
        
    }
}