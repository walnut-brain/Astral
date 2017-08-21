using Astral.Configuration.Settings;
using Astral.Lavium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration
{
    public class EndpointConfig : EndpointLevelConfigBase
    {
        private ILogger<EndpointConfig> _logger;
        
        internal EndpointConfig(LawBook config) : base(config)
        {
            _logger = GetLogger<EndpointConfig>();
        }

    }
}