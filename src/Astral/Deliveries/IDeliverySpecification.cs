using System.Net.Mime;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;

namespace Astral.Deliveries
{
    public interface IDeliverySpecification
    {
        string System { get; }
        string TransportTag { get; }
        string Service { get; }
        string Endpoint { get; }
        ContentType ContentType { get; }
        Encode TypeEncoder { get; }
        SerializeProvider<byte[]> SerializeProvider { get; }
    }
}