using Astral.Delivery;

namespace Astral.Configuration.Settings
{
    public interface IAfterDeliveryPolicy
    {
        void Execute(IDeliveryCloseOperations onClose);
    }
}