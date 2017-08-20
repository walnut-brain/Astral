using System.Data;

namespace Astral.Data
{
    public interface IUnitOfWorkProvider<out T>
        where T : IUnitOfWork
    {
        T BeginWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}