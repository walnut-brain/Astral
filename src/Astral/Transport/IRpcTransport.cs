using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface IRpcTransport
    {
        Task<Payload<byte[]>> Call<TRequest>(EndpointConfig config, Lazy<TRequest> request, Payload<byte[]> payload, TimeSpan timeout);
        IDisposable Handle<TRequest>(EndpointConfig config, Func<Payload<byte[]>, CancellationToken, Task<Payload<byte[]>>> handler);
    }
}