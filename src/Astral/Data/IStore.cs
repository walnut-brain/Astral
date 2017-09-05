using System.Data;

namespace Astral.Data
{
    public interface IStore<TStore>
        where TStore : IStore<TStore>
    {
        IStoreWork BeginWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}