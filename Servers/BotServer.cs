using Shared;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class BotServer : GenericServer<BotContext, BotConfig, BotParameters>
{
    public BotServer(IListener<BotContext, BotParameters> listener, IMiddleware<BotContext> middleware,
        ILogger<BotServer> logger, BotConfig? config = null) : base(listener,
        middleware, logger, config)
    {
    }
}