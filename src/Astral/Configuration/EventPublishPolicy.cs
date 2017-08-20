using System.Runtime.Serialization;
using LanguageExt;
using Polly;

namespace Astral.Configuration
{
    public class EventPublishPolicy : NewType<EventPublishPolicy, Policy>
    {
        public EventPublishPolicy(Policy value) : base(value)
        {
        }
    }
}