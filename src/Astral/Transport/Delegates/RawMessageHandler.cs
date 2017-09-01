using System.Threading;
using System.Threading.Tasks;
using Astral.Payloads;
using Astral.Serialization;

namespace Astral.Transport
{
    public delegate Task<Acknowledge> RawMessageHandler(PayloadBase<byte[]> rawMessage, EventContext context,
        CancellationToken token);
}