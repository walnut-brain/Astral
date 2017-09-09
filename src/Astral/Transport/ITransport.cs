using System;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface ITransport : IRpcTransport
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointSpecification specification, PublishOptions options);
        IDisposable Subscribe(EndpointSpecification specification, RawMessageHandler handler, EventListenOptions options);
    }
}