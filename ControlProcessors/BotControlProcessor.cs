using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProcessors;

public class BotControlProcessor: BaseControlProcessor
{
    private readonly IBot _bot;

    public BotControlProcessor(string name, IBot bot, ILogger logger) : base(name, logger, ControlProcessorType.Bot)
    {
        _bot = bot;
    }

    protected override void StartInternal(AppConfig config)
    {
        _bot.Start(config.BotConfig.ChatId);
    }

    protected override void StopInternal()
    {
        _bot.Stop();
    }
}