using System;

namespace Astral.Data
{
    public static class Extensions
    {
        public static void RegisterAfterSuccessCommit(this IAfterCommit afterCommit, Action action)
            => afterCommit.RegisterAfterCommit(p =>
            {
                if (p) action();
            });
    }
}