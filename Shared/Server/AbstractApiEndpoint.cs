using Shared.Logging.Interfaces;

namespace Shared.Server;

public abstract class AbstractApiEndpoint: AbstractEndpoint
{
    public string ApiVersion = string.Empty;
    
    protected AbstractApiEndpoint(ILogger logger) : base(logger)
    {
    }
}