using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Payloads.Serialization
{
    public class Serialization<TFormat> 
    {
        public Serialization(SerializeProvider<TFormat> serialize, DeserializeProvider<TFormat> deserialize)
        {
            Serialize = serialize;
            Deserialize = deserialize;
        }

        public SerializeProvider<TFormat> Serialize { get; }
        public DeserializeProvider<TFormat> Deserialize { get; }
        
    }
}