using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;

namespace Shared.Server;

public abstract class AbstractEndpoint
{
    protected readonly ILogger Logger;

    public string ApiVersion = "v1";
    public bool IsStaticFiles = false;

    protected AbstractEndpoint(ILogger logger)
    {
        Logger = logger;
    }

    public abstract void ProcessRequest(IContext context);
}