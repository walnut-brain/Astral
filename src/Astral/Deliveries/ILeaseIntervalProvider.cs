using System;

namespace Astral.Deliveries
{
    public interface ILeaseIntervalProvider
    {
        TimeSpan GetLeaseInterval<TStore>();
    }
}