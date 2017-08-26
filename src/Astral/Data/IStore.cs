using System.Data;
using System.Threading.Tasks;

namespace Astral.Data
{
    public interface IStore<T>
        where T : IStore<T>
    {
        Task<IUnitOfWork> BeginWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}