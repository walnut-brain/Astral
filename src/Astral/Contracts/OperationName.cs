namespace Astral.Contracts
{
    public class OperationName
    {
        public OperationName(string serviceName, string endpointName)
        {
            ServiceName = serviceName;
            EndpointName = endpointName;
        }

        public string ServiceName { get; }
        public string EndpointName { get; }
    }
}