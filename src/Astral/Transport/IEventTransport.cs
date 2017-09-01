using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Payloads;
using Astral.Serialization;

namespace Astral.Transport
{
    public interface IEventTransport : ITransport
    {
        Func<Task> PreparePublish<T>(EndpointConfig config, T message, PayloadBase<byte[]> payload,
            PublishOptions options);

        IDisposable Subscribe(EndpointConfig config,
            RawMessageHandler handler,
            EventListenOptions options);
    }
}