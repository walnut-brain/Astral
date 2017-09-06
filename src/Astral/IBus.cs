using System;

namespace Astral
{
    public interface IBus : IDisposable
    {
        IBusService<T> Service<T>() where T : class;
    }
}