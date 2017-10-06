using System;
using System.Net.Mime;
using RabbitLink.Messaging;
using RabbitLink.Serialization;

namespace RabbitLink.Services
{
    public interface IPayloadManager
    {
        byte[] Serialize<T>(ContentType defaultContentType, T body, LinkMessageProperties props);
        object Deserialize(ILinkMessage<byte[]> message, Type awaitedType);
    }
}