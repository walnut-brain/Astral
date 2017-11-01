namespace Astral.Logging
{
    public interface ILogFactoryProvider
    {
        ILogFactory LogFactory { get; }
    }
}