using Bots.Telegram;
using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: IBot
{
    private ILogger _logger;

    private const int RefreshTime = 1_000;
    private AppConfig _currentConfig;

    private TelegramBotApiWrapper _wrapper;
    private readonly CommandsExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    private CancellationTokenSource _cts;
    private CancellationToken _token;

    public TelegramBot(ILogger logger, AppConfig config, CommandsExecutor executor)
    {
        _logger = logger;
        _executor = executor;
        _currentConfig = config;

        _cts = new CancellationTokenSource();
        _token = _cts.Token;
    }

    public void Start(int chatId)
    {
        _wrapper = new TelegramBotApiWrapper(_currentConfig.BotConfig.ApiUri, _currentConfig.BotConfig.ApiKey);
        _cts = new CancellationTokenSource();
        _token = _cts.Token;

        Listen(chatId, _token);
    }

    private async void Listen(int chatId, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var response = await _wrapper.GetUpdates();

            if(!response.Ok)
                continue;

            var messages = response.Result
                .Where(x => x.Message?.Chat?.Id == chatId && (DateTime.Now - x.Message.ParsedDate).Minutes < 5)
                .Select(x => x.Message?.Text).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var responses = _executor.Execute(messages);

            foreach (var res in responses.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                await _wrapper.SendResponse(chatId, res!, _buttons);
            }

            await Task.Delay(RefreshTime, token);
        }
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    public bool IsRunning => _cts.IsCancellationRequested;
    public int GetChatId() => _currentConfig.BotConfig.ChatId;
}