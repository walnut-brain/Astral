namespace Astral.Configuration.Settings
{
    public sealed class EndpointKind : Fact<EndpointType>
    {
        public EndpointKind(EndpointType value) : base(value)
        {
        }
    }
}