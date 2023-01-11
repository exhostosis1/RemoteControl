using Shared.Config;

namespace Shared.ControlProcessor;

public abstract class BotProcessor : GenericControlProcessor<BotConfig>
{
    protected BotProcessor(BotConfig? config = null): base(config)
    {
        DefaultConfig = new BotConfig
        {
            ApiUri = "https://api.telegram.org/bot"
        };
    }
}