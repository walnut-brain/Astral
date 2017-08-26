using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral
{
    public static class Astral
    {
        public static IEventListener<TEvent> CreateEventHandler<TEvent>(
            Func<TEvent, EventContext, CancellationToken, Task> handler)
        {
            return new DelegateEventListener<TEvent>(handler);
        }

        private class DelegateEventListener<TEvent> : IEventListener<TEvent>
        {
            private readonly Func<TEvent, EventContext, CancellationToken, Task> _action;

            public DelegateEventListener(Func<TEvent, EventContext, CancellationToken, Task> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public Task Handle(TEvent @event, EventContext context, CancellationToken token)
            {
                return _action(@event, context, token);
            }
        }
    }
}