using Shared;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class BotServer : GenericServer<BotContext, BotConfig, BotParameters>
{
    public BotServer(IBotListener listener, IBotMiddlewareChain middleware,
        ILogger<BotServer> logger) : base(listener,
        middleware, logger)
    {
    }
}