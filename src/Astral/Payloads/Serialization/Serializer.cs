namespace Astral.Payloads.Serialization
{
    public class Serializer<TFormat> 
    {
        public Serializer(SerializeProvider<TFormat> serialize, DeserializeProvider<TFormat> deserialize)
        {
            Serialize = serialize;
            Deserialize = deserialize;
        }

        public SerializeProvider<TFormat> Serialize { get; }
        public DeserializeProvider<TFormat> Deserialize { get; }
        
        
    }
}