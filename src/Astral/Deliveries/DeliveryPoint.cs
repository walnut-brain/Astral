namespace Astral.Deliveries
{
    public class DeliveryPoint
    {
        public DeliveryPoint(string system, string transportTag, string service, string endpoint)
        {
            System = system;
            TransportTag = transportTag;
            Service = service;
            Endpoint = endpoint;
        }

        public string System { get; }
        public string TransportTag { get; }
        public string Service { get; }
        public string Endpoint { get; }
    }
}