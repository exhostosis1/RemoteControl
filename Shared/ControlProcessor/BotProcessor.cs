using Shared.Config;
using Shared.Logging.Interfaces;

namespace Shared.ControlProcessor;

public abstract class BotProcessor : GenericControlProcessor<BotConfig>
{
    protected BotProcessor(ILogger logger, BotConfig? config = null) : base(logger, config)
    {
        DefaultConfig = new BotConfig
        {
            ApiUri = "https://api.telegram.org/bot"
        };
    }
}