using Shared.Config;
using Shared.Logging.Interfaces;

namespace Shared.ControlProcessor;

public abstract class ServerProcessor : GenericControlProcessor<ServerConfig>
{
    protected ServerProcessor(ILogger logger, ServerConfig? config = null) : base(logger, config)
    {
        DefaultConfig.Scheme = "http";
        DefaultConfig.Host = "localhost";
        DefaultConfig.Port = 1488;
    }
}