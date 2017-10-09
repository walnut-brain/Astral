using System;
using System.Net.Mime;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    /// <summary>
    /// Payload manager
    /// </summary>
    public interface IPayloadManager
    {
        /// <summary>
        /// serialize payload and set message properties
        /// </summary>
        /// <param name="defaultContentType">serialization content type</param>
        /// <param name="body">message</param>
        /// <param name="props">message properties</param>
        /// <typeparam name="T">message type</typeparam>
        /// <returns>serialized message</returns>
        byte[] Serialize<T>(ContentType defaultContentType, T body, LinkMessageProperties props);
        
        /// <summary>
        /// deserialize payload use message properties
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="awaitedType">type of message to deserialize</param>
        /// <returns>deserialized value, can be another type then awaitedType</returns>
        object Deserialize(ILinkMessage<byte[]> message, Type awaitedType);
    }
}