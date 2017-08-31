using System.Threading;
using System.Threading.Tasks;
using Astral.Core;
using Astral.Serialization;

namespace Astral.Transport
{
    public delegate Task<Acknowledge> RawMessageHandler(Payload<byte[]> rawMessage, EventContext context,
        CancellationToken token);
}