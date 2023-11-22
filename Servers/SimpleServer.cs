using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer(IWebListener listener, IWebMiddlewareChain middleware,
    ILogger<SimpleServer> logger) : GenericServer<WebContext, WebConfig, WebParameters>(
    listener, middleware, logger)
{
}