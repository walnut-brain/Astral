namespace Astral.Exceptions
{
    public class TransportNotFoundException : PermanentException
    {
        public TransportNotFoundException(string tag) : base(
             string.IsNullOrWhiteSpace(tag) ? "Default transport not found" : $"Transport {tag} not found")
        
        {
            
            Tag = tag;
        }

        
        public string Tag { get; }
    }
}