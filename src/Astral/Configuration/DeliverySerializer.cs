using LanguageExt;
using WalnutBrain.Bus.Serialization;

namespace Astral.Configuration
{
    public class DeliverySerialize : NewType<DeliverySerialize, ISerialize<string>>
    {
        public DeliverySerialize(ISerialize<string> value) : base(value)
        {
        }
    }
}