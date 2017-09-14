using System;
using Astral.Transport;

namespace Astral.Configuration.Settings
{
    public sealed class ResponseToSetting : Fact<ChannelKind.RespondableChannel>
    {
        public ResponseToSetting(ChannelKind.RespondableChannel value) : base(value)
        {
        }
    }
}