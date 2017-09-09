using System;
using Astral.Specifications;

namespace Astral.Transport
{
    public delegate IDisposable RawMessageSubscribe(EndpointSpecification specification, RawMessageHandler handler,
        EventListenOptions options);

}