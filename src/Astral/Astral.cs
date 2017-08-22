using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Astral
{
    public static class Astral
    {
        private static Policy _defaultEventPublishPolicy = 
            Policy.NoOpAsync();
        
        private static readonly object DefaultEventPublishPolicyLocker = new object();
        
        private static Policy _defaultDeliveryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, p => TimeSpan.FromSeconds(Math.Pow(2, p)));
        private static readonly object DefaultDeliveryPolicyLocker = new object();

        public static Policy DefaultEventPublishPolicy
        {
            get
            {
                lock (DefaultEventPublishPolicyLocker)
                {
                    return _defaultEventPublishPolicy;    
                }
                
            }
            set
            {
                if (value != null)
                    value = Policy.NoOpAsync();
                lock (DefaultEventPublishPolicyLocker)
                {
                    _defaultEventPublishPolicy = value;
                }
            }
        }

        public static Policy DefaultDeliveryPolicy
        {
            get
            {
                lock (DefaultEventPublishPolicyLocker)
                {
                    return _defaultDeliveryPolicy;    
                }
            }
            set
            {
                if (value == null)
                    value = Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(5, p => TimeSpan.FromSeconds(Math.Pow(2, p)));
                _defaultDeliveryPolicy = value; }
        }

        public static IEventHandler<TEvent> CreateEventHandler<TEvent>(
            Func<TEvent, EventContext, CancellationToken, Task> handler)
            => new DelegateEventHandler<TEvent>(handler);

        private class DelegateEventHandler<TEvent> : IEventHandler<TEvent>
        {
            private readonly Func<TEvent, EventContext, CancellationToken, Task> _action;

            public DelegateEventHandler(Func<TEvent, EventContext, CancellationToken, Task> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public Task Handle(TEvent @event, EventContext context, CancellationToken token)
                => _action(@event, context, token);
        }
    }
}