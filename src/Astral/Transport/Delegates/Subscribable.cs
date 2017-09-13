using System;
using Astral.Specifications;

namespace Astral.Transport
{
    public delegate IDisposable Subscribable(RawMessageHandler channel);
}