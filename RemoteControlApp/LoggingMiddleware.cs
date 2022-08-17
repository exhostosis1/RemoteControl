using Shared.Interfaces.Logging;
using Shared.Interfaces.Web;

namespace RemoteControlApp
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LoggingMiddleware : IMiddleware
    {
        private readonly HttpEventHandler _next;
        private readonly ILogger _logger;

        public LoggingMiddleware(HttpEventHandler next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public void ProcessRequest(IContext context)
        {
            _logger.LogInfo(context.Request.Path);

            _next(context);
        }
    }
}
