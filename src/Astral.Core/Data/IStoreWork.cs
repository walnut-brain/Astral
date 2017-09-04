using System;

namespace Astral.Data
{
    public interface IStoreWork : IDisposable, IObservable<ValueTuple>
    {
        void Commit();
    }
}