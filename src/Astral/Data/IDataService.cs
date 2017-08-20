namespace Astral.Data
{
    public interface IDataService<out T>
        where T : IUnitOfWork
    {
        T UnitOfWork { get; }
    }
}