using System;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface ITransport 
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, PublishOptions options);
        IDisposable Subscribe(ChannelConfig config, RawMessageHandler handler);
    }
}