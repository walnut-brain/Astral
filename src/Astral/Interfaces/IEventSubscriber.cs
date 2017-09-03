using System;
using System.Linq.Expressions;

namespace Astral
{
    public interface IEventSubscriber<TService>
    {
        IDisposable Subscribe<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener,
            EventListenOptions options = null);
    }
}