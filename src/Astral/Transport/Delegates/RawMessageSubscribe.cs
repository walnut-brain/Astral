using System;
using Astral.Configuration.Configs;

namespace Astral.Transport
{
    public delegate IDisposable RawMessageSubscribe(EndpointConfig config, RawMessageHandler handler,
        EventListenOptions options);

}