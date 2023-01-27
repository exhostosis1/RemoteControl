using Shared;
using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer : GenericServer<WebContext, WebConfig, WebParameters>
{
    public SimpleServer(IListener<WebContext, WebParameters> listener, IMiddleware<WebContext> middleware,
        ILogger<SimpleServer> logger, WebConfig? config = null) : base(
        listener, middleware, logger, config)
    {
    }
}