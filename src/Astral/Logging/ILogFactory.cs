using System;

namespace Astral.Logging
{
    public interface ILogFactory
    {
        ILog CreateLog(string category);
        ILog CreateLog<T>();
        ILog CreateLog(Type type);
    }
}