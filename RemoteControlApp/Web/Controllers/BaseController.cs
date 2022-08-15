using Shared.Interfaces.Logging;

namespace RemoteControlApp.Web.Controllers
{
    public abstract class BaseController
    {
        protected readonly ILogger Logger;

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }
    }
}
