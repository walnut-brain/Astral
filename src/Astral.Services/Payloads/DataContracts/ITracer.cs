using System;

namespace Astral.Payloads.DataContracts
{
    public interface ITracer 
    {
        void Write(string message);
        IDisposable Scope(string name, ushort offset = 4);
    }
}