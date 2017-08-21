using Astral.Lavium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration
{
    public class MessageConfig : EndpointLevelConfigBase
    {
        private ILogger<MessageConfig> _logger;
        
        internal MessageConfig(LawBook config) : base(config)
        {
            _logger = GetLogger<MessageConfig>();
        }

    }
}