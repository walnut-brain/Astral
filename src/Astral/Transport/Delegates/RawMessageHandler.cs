using System.Threading;
using System.Threading.Tasks;
using Astral.Serialization;

namespace Astral.Transport
{
    public delegate Task<Acknowledge> RawMessageHandler(Serialized<byte[]> rawMessage, EventContext context,
        CancellationToken token);
}