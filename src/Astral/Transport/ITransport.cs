using System;
using Astral.Configuration.Configs;
using Astral.Payloads;

namespace Astral.Transport
{
    public interface ITransport : IRpcTransport
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, PublishOptions options);
    }

    public interface IRpcTransport
    {
    }
}