using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware : AbstractMiddleware<WebContext>
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger, IWebMiddleware? next = null) : base(next)
    {
        _logger = logger;
    }

    public override void ProcessRequest(WebContext context)
    {
        _logger.LogInfo(context.WebRequest.Path);

        Next?.ProcessRequest(context);

        _logger.LogInfo($"{context.WebResponse.StatusCode}\n{context.WebResponse.ContentType}\n{context.WebResponse.Payload}");
    }
}