using System;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Payloads;


namespace Astral.Transport
{
    public interface IEventTransport : ITransport
    {
        

        IDisposable Subscribe(EndpointConfig config,
            RawMessageHandler handler,
            EventListenOptions options);
    }
}