using Shared.Interfaces.Logging;

namespace RemoteControlApp.Web.Controllers
{
    public abstract class BaseController
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }
    }
}
