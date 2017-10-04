using System.Data;

namespace ServiceLink
{
    /// <summary>
    /// Provide interface for starting unit of work
    /// </summary>
    public interface IUnitOfWorkProvider<T> : IAfterCommit
    {
        /// <summary>
        /// Start unit of work
        /// </summary>
        /// <param name="isolationLevel">level of isolation</param>
        /// <returns>unit of work</returns>
        IUnitOfWork BeginWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}