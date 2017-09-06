using System;
using Astral.Configuration.Configs;
using Astral.Payloads;

namespace Astral.Transport
{
    public interface ITransport : IRpcTransport
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, PublishOptions options);
        IDisposable Subscribe(EndpointConfig config, RawMessageHandler handler, EventListenOptions options);
    }
}