using Shared.Interfaces;
using Shared.Interfaces.Logging;
using Shared.Interfaces.Web;

namespace RemoteControlApp.Middleware
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

    public static partial class AppExtensions
    {
        public static IRemoteControlApp UseLoggingMiddleware(this IRemoteControlApp app)
        {
            app.UseMiddleware<LoggingMiddleware>();
            return app;
        }
    }
}
