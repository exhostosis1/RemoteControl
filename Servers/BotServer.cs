using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class BotServer(IBotListener listener, IBotMiddlewareChain middleware,
    ILogger<BotServer> logger) : GenericServer<BotContext, BotConfig, BotParameters>(listener,
    middleware, logger)
{
}