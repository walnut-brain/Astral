using System;

namespace WalnutBrain.Data
{
    public interface IRegisterAfterCommit
    {
        /// <summary>
        ///     Register action for executing after real succesess commit
        /// </summary>
        /// <param name="action">Action to execute</param>
        void RegisterAfterCommit(Action action);
    }
}