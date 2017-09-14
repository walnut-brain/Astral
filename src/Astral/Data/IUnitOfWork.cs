using System;

namespace Astral.Data
{
    public interface IUnitOfWork<TStore> : IDisposable, IWorkResultReporter
    {
        T GetService<T>() where T : IStoreService<TStore>;
        void Commit();
        
    }
}