namespace Astral.Payloads
{
    public interface IPayloadSerializer
    {
        bool SupportContentType(string contentType);
    }
}