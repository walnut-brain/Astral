using System.Threading;
using System.Threading.Tasks;
using Astral.Internals;
using Astral.Payloads;

namespace Astral.Transport
{
    public delegate Task<Acknowledge> RawMessageHandler(Payload<byte[]> rawMessage, MessageContext context,
        CancellationToken token);
}