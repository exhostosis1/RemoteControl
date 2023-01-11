using Shared.Config;

namespace Shared.ControlProcessor;

public abstract class ServerProcessor : GenericControlProcessor<ServerConfig>
{
    protected ServerProcessor(ServerConfig? config = null) : base(config)
    {
        DefaultConfig.Scheme = "http";
        DefaultConfig.Host = "localhost";
        DefaultConfig.Port = 1488;
    }
}