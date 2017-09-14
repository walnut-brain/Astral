using System;

namespace Astral.Data
{
    public interface IWorkResultReporter
    {
        IObservable<ValueTuple> WorkResult { get; }
    }
}