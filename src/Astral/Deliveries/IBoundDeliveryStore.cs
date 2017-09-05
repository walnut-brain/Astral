using Astral.Data;

namespace Astral.Deliveries
{
    public interface IBoundDeliveryStore<TStore> : IDeliveryStore<TStore>
        where TStore : IStore<TStore>, IDeliveryStore<TStore>
    {
        
    }
}