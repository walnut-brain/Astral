using System;
using Astral.Transport;

namespace Astral.Configuration.Settings
{
    public sealed class ResponseToSetting : Fact<ChannelKind>
    {
        public ResponseToSetting(ChannelKind value) : base(value)
        {
            if(!(value is ChannelKind.IResponseTo))
                throw new ArgumentOutOfRangeException("Channel must be responsable");
        }
    }
}