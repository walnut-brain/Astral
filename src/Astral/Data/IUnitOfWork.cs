﻿using System;
using System.Threading.Tasks;
using LanguageExt;
using WalnutBrain.Data;

namespace Astral.Data
{
    /// <inheritdoc />
    /// <summary>
    ///     Unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable 
    {
        /// <summary>
        ///     Commit changes. When not called, Dispose must rollback
        /// </summary>
        Task Commit();
    }
}