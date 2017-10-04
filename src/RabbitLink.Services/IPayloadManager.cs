using System;
using System.Net.Mime;
using RabbitLink.Serialization;

namespace RabbitLink.Services
{
    public interface IPayloadManager
    {
        ILinkSerializer GetSerializer(ContentType defaultContentType);
    }
}