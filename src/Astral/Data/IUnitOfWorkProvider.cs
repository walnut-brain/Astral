using System.Data;
using System.Threading.Tasks;

namespace Astral.Data
{
    public interface IUnitOfWorkProvider<T>
        where T : IUnitOfWork
    {
        Task<T> BeginWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}