using Shared.Enums;
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
            _logger.Log(context.Request.Path, LoggingLevel.Info);
        }
    }
}
