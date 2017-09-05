namespace Astral.Exceptions
{
    public class TransportNotFoundException : PermanentException
    {
        public TransportNotFoundException(bool isRpc, string tag) : base(
            isRpc ? 
                string.IsNullOrWhiteSpace(tag) ? "Default rpc transport not found" : $"Rpc transport {tag} not found"
                : string.IsNullOrWhiteSpace(tag) ? "Default transport not found" : $"Transport {tag} not found")
        
        {
            IsRpc = isRpc;
            Tag = tag;
        }

        public bool IsRpc { get; }
        public string Tag { get; }
    }
}