using Astral.Contracts;

namespace RabbitLink.Services
{
    public interface ILinkEventPublisher : IEventPublisher
    {
        ILinkEventPublisher DeclarePassive(bool value);
        ILinkEventPublisher ConfirmMode(bool value);
        ILinkEventPublisher Named(string name);    
    }

    public interface ILinkEventPublisher<TService, TEvent> : IEventPublisher<TService, TEvent>
        where TEvent : class    
    {
        ILinkEventPublisher<TService, TEvent> DeclarePassive(bool value);
        ILinkEventPublisher<TService, TEvent> ConfirmMode(bool value);
        ILinkEventPublisher<TService, TEvent> Named(string name);
    }
}