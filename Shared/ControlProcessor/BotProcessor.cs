using Shared.Config;

namespace Shared.ControlProcessor;

public abstract class BotProcessor : GenericControlProcessor<BotConfig>
{
    protected BotProcessor(BotConfig? config = null): base(config)
    {
        DefaultConfig.ApiUri = "https://api.telegram.org/bot";
    }
}