using System;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Core;
using Astral.Serialization;

namespace Astral.Transport
{
    public delegate Func<Task> PreparePublish<in TEvent>(EndpointConfig config, TEvent message,
        Payload<byte[]> payload, PublishOptions options);

}