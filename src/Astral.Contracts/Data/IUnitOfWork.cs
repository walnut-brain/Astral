using System;

namespace Astral.Data
{
    /// <summary>
    /// Unit of work 
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAfterCommit
    {
        /// <summary>
        /// Commit changes. When not called, Dispose must rollback
        /// </summary>
        void Commit();
    }
}