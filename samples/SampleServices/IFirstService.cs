using System;
using Astral.Markup;
using Astral.Markup.RabbitMq;

namespace SampleServices
{
    [Owner("test")]
    [Service("first")]
    public interface IFirstService
    {
        [Endpoint("event")]
        EventHandler<EventContract> Event { get; }
        
        [Exchange("test.call.exchange", Durable = false)]
        [Endpoint("test.call")]
        Func<int, int> Call { get; }
    }
}