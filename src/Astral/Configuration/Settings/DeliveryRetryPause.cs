using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class DeliveryRetryPause : NewType<DeliveryRetryPause, Func<ushort, TimeSpan>>
    {
        public DeliveryRetryPause(Func<ushort, TimeSpan> value) : base(value)
        {
        }
    }
}