using System;

namespace Astral
{
    public class RpcFailException : Exception
    {
        public RpcFailException(string message, string kind) : base(message)
        {
            Kind = kind;
        }

        public string Kind { get; } 
    }
}