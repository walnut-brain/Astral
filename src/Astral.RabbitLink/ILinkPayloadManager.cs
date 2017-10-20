using System;
using System.Collections.Generic;
using System.Net.Mime;
using Astral.Schema;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    /// <summary>
    /// Payload manager
    /// </summary>
    public interface ILinkPayloadManager
    {
        /// <summary>
        /// serialize payload and set message properties
        /// </summary>
        /// <param name="defaultContentType">serialization content type</param>
        /// <param name="body">message</param>
        /// <param name="props">message properties</param>
        /// <param name="knownTypes">known types schemas</param>
        /// <typeparam name="T">message type</typeparam>
        /// <returns>serialized message</returns>
        byte[] Serialize<T>(ContentType defaultContentType, T body, LinkMessageProperties props, IReadOnlyCollection<ITypeSchema> knownTypes);

        /// <summary>
        /// deserialize payload use message properties
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="knownTypes">known types schemas</param>
        /// <returns>deserialized value, can be another type then T</returns>
        object Deserialize<T>(ILinkMessage<byte[]> message, IReadOnlyCollection<ITypeSchema> knownTypes);
    }
}