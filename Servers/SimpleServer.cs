using Microsoft.Extensions.Logging;
using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Server;

namespace Servers;

public class SimpleServer(IWebListener listener, IWebMiddlewareChain middleware,
    ILogger logger) : GenericServer<WebContext, WebConfig, WebParameters>(
    listener, middleware, logger)
{
}