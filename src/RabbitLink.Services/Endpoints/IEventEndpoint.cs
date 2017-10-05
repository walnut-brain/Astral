namespace RabbitLink.Services
{
    public interface IEventEndpoint<TService, TEvent>
        where TEvent : class
    {
        ILinkEventPublisher<TService, TEvent> Publisher { get; }
        ILinkEventConsumer<TService, TEvent> Consumer { get; }
    }

    
}