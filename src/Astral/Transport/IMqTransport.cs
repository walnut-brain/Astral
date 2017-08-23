using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Serialization;

namespace Astral.Transport
{
    public interface IMqTransport : ITransport
    {
        Func<Task> PreparePublish<T>(EndpointConfig config, T message, Serialized<byte[]> serialized,
            PublishOptions options);

         IDisposable Subscribe(EndpointConfig config,
            Func<Serialized<byte[]>, EventContext, CancellationToken, Task<Acknowledge>> handler,
            EventSubscribeOptions options);
    }
}