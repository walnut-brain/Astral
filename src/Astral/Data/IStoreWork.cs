using System;

namespace Astral.Data
{
    public interface IStoreWork : IDisposable
    {
        /// <summary>
        /// Must complete on commit or onError when commit error or rollback
        /// </summary>
        IObservable<ValueTuple> CommitEvents { get; }
        void Commit();
    }
}