using Shared.Interfaces.Logging;
using Shared.Interfaces.Web;

namespace RemoteControlApp.Web.Middleware
{
    public class LoggingMiddleware: BaseMiddleware
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        protected override void ProcessRequestInternal(IContext context)
        {
            _logger.LogInfo(context.Request.Path);
        }
    }
}
