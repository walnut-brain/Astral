using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Core;
using Astral.Serialization;

namespace Astral.Transport
{
    public interface IEventTransport : ITransport
    {
        Func<Task> PreparePublish<T>(EndpointConfig config, T message, Payload<byte[]> payload,
            PublishOptions options);

        IDisposable Subscribe(EndpointConfig config,
            RawMessageHandler handler,
            EventListenOptions options);
    }
}