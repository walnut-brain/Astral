using System;

namespace Astral.Data
{
    public interface IAfterCommit
    {
        /// <summary>
        /// Register action for executing after real commit, successefulity as argument (true id success commit, false for error)
        /// </summary>
        /// <param name="action">Action to execute</param>
        void RegisterAfterCommit(Action<bool> action);
    }
}