using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware : AbstractMiddleware
{
    public LoggingMiddleware(ILogger logger, HttpEventHandler? next = null) : base(logger, next)
    {
    }

    public override void ProcessRequest(IContext context)
    {
        Logger.LogInfo(context.Request.Path);

        Next?.Invoke(context);

        Logger.LogInfo($"{context.Response.StatusCode}\n{context.Response.ContentType}\n{context.Response.Payload}");
    }
}