using Bots.Telegram;
using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: IBot
{
    private BotApiWrapper _wrapper;
    private ILogger _logger;

    public TelegramBot(ILogger logger, AppConfig config)
    {
        _logger = logger;

        _wrapper = new BotApiWrapper(config.BotConfig.ApiUri, config.BotConfig.ApiKey);
    }

    public void Start(string apiKey, int chatId)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public bool IsRunning { get; }
    public int GetChatId()
    {
        throw new NotImplementedException();
    }
}