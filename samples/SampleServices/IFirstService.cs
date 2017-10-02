using System;

namespace SampleServices
{
    public interface IFirstService
    {
        EventHandler<EventContract> Event { get; }
    }
}