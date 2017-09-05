using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Payloads;

namespace Astral.Transport
{
    public delegate Task PayloadSender<T>(Lazy<T> message, Payload<byte[]> payload, CancellationToken cancellation);

}