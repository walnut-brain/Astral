using System;
using System.Threading.Tasks;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Transport
{
    public delegate PayloadSender<TEvent> PreparePublish<TEvent>(EndpointConfig config, PublishOptions options);

}