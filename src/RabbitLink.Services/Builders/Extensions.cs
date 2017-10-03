using RabbitLink.Builders;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    public static partial class Extensions
    {
        public static IServiceLinkBuilder ToServiceLink(this ILinkBuilder linkBuilder, IPayloadManager payloadManager, 
            IDescriptionFactory descriptionFactory, string serviceName)
            => new ServiceLinkBuilder(linkBuilder.AppId(serviceName), payloadManager, descriptionFactory, serviceName);
    }
}