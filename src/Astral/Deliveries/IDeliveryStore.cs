namespace Astral.Deliveries
{
    public interface IDeliveryStore<TStore> 
        where TStore : IDeliveryStore<TStore>

    {
        IDeliveryDataService<TStore> DeliveryService { get; }
    }
}