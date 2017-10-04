using System;

namespace Astral.Disposables
{
    public interface IDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}