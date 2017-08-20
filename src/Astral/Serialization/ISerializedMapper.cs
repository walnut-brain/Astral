namespace WalnutBrain.Bus.Serialization
{
    public interface ISerializedMapper<TF1, TF2>
    {
        Serialized<TF2> Map(Serialized<TF1> serialized);
    }
}