﻿using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer : GenericServer<WebContext, WebConfig, WebParameters>
{
    public SimpleServer(IWebListener listener, IWebMiddlewareChain middleware,
        ILogger<SimpleServer> logger) : base(
        listener, middleware, logger)
    {
    }
}