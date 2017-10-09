namespace Astral.Logging
{
    public interface ILogFactory
    {
        ILog CreateLog(string category);
    }
}