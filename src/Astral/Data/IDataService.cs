namespace Astral.Data
{
    public interface IDataService<T>
        where T : IStore<T>
    {
        T Store { get; }
    }
}