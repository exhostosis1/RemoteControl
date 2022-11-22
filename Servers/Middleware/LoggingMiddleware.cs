using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly HttpEventHandler? _next;
        private readonly ILogger _logger;

        public LoggingMiddleware(HttpEventHandler next, ILogger logger): this(logger)
        {
            _next = next;
        }

        public LoggingMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(IContext context)
        {
            _logger.LogInfo(context.Request.Path);

            _next?.Invoke(context);

            _logger.LogInfo($"{context.Response.StatusCode}\n{context.Response.ContentType}\n{context.Response.Payload}");
        }
    }
}
