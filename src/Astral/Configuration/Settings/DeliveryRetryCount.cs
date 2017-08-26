using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class DeliveryRetryCount : NewType<DeliveryRetryCount, ushort>
    {
        public DeliveryRetryCount(ushort value) : base(value)
        {
        }
    }
}