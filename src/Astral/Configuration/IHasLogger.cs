using Microsoft.Extensions.Logging;

namespace Astral.Configuration
{
    public interface IHasLogger
    {
        ILogger Logger { get; }
    }
}