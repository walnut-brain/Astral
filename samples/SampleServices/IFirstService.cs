using System;
using Astral.Markup;

namespace SampleServices
{
    [Owner("test")]
    [Service("first")]
    public interface IFirstService
    {
        [Endpoint("event")]
        EventHandler<EventContract> Event { get; }
    }
}