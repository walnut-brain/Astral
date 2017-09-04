using System;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Payloads;

namespace Astral.Transport
{
    public delegate PayloadSender<TEvent> PreparePublish<TEvent>(EndpointConfig config, PublishOptions options);

}