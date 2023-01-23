using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware : AbstractMiddleware<HttpContext>
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger, AbstractMiddleware<HttpContext>? next = null) : base(next)
    {
        _logger = logger;
    }

    public override void ProcessRequest(HttpContext context)
    {
        _logger.LogInfo(context.HttpRequest.Path);

        Next?.ProcessRequest(context);

        _logger.LogInfo($"{context.HttpResponse.StatusCode}\n{context.HttpResponse.ContentType}\n{context.HttpResponse.Payload}");
    }
}