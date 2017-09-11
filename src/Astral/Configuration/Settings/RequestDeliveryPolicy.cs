using Astral.Deliveries;
using FunEx.Monads;

namespace Astral.Configuration.Settings
{
    public sealed class RequestDeliveryPolicy : Fact<DeliveryAfterCommit>
    {
        public RequestDeliveryPolicy(DeliveryAfterCommit value) : base(value)
        {
        }
    }
}