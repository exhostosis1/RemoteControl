using Shared.Interfaces.Logging;
using Shared.Interfaces.Web;

namespace WebApiProvider.Controllers
{
    public abstract class BaseController: IController
    {
        protected readonly ILogger Logger;

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }
    }
}
