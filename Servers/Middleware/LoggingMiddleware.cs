using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IWebMiddleware
{
    public event EventHandler<WebContext>? OnNext;

    public void ProcessRequest(object? _, WebContext context)
    {
        logger.LogInfo(context.WebRequest.Path);

        OnNext?.Invoke(null, context);

        logger.LogInfo($"{context.WebResponse.StatusCode}\n{context.WebResponse.ContentType}\n{context.WebResponse.Payload}");
    }
}