using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;

namespace Shared.Server;

public abstract class AbstractMiddleware
{
    protected readonly HttpEventHandler? Next;
    protected readonly ILogger Logger;

    protected AbstractMiddleware(ILogger logger, HttpEventHandler? next = null)
    {
        Logger = logger;
        Next = next;
    }

    public abstract void ProcessRequest(IContext context);
}