using System;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Payloads;
using Astral.Serialization;

namespace Astral.Transport
{
    public delegate Func<Task> PreparePublish<in TEvent>(EndpointConfig config, TEvent message,
        PayloadBase<byte[]> payload, PublishOptions options);

}