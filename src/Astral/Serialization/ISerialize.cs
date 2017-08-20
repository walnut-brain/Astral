namespace WalnutBrain.Bus.Serialization
{
    public interface ISerialize<TFormat>
    {
        Serialized<TFormat> Serialize(string typeCode, object obj);
    }
}