using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    public interface IServiceLink : ILink
    {
        IDescriptionFactory DescriptionFactory { get; }
        IPayloadManager PayloadManager { get; }
        IServiceBuilder<TService> Service<TService>();
        string ServiceName { get; }
    }
}