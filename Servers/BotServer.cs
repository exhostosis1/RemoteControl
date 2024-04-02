using Microsoft.Extensions.Logging;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.Listener;
using Shared.Server;

namespace Servers;

public class BotServer(IBotListener listener, IBotMiddlewareChain middleware,
    ILogger logger) : GenericServer<BotContext, BotConfig, BotParameters>(listener,
    middleware, logger)
{
}