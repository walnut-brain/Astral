using Astral.Links;

namespace RabbitLink.Services
{
    public interface ILinkEventPublisher<TService, TEvent> : IEventPublisher<TService, TEvent>
        where TEvent : class    
    {
        ILinkEventPublisher<TService, TEvent> DeclarePassive(bool value);
        ILinkEventPublisher<TService, TEvent> ConfirmMode(bool value);
        ILinkEventPublisher<TService, TEvent> Named(string name);
    }
}