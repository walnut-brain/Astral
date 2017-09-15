using System;
using Astral.Transport;

namespace Astral.Configuration.Settings
{
    public sealed class ResponseTo : Fact<ChannelKind.RespondableChannel>
    {
        public ResponseTo(ChannelKind.RespondableChannel value) : base(value)
        {
        }
    }
}