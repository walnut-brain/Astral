using System;

namespace Astral
{
    public class EventPublishOptions
    {
        public EventPublishOptions(TimeSpan? eventTtl)
        {
            EventTtl = eventTtl;
        }

        public TimeSpan? EventTtl { get; }
    }
}