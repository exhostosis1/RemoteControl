using Bots.Telegram;
using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: IControlProcessor
{
    private ILogger _logger;
    public ControlPocessorEnum Status { get; private set; }= ControlPocessorEnum.Stopped;
    public string Name { get; set; } = "telegram bot";
    public ControlProcessorType Type => ControlProcessorType.Bot;
    public string Info => _currentConfig.BotConfig.ChatId.ToString();

    private const int RefreshTime = 1_000;
    private AppConfig _currentConfig;
    
    private readonly CommandsExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    private CancellationTokenSource _cts;
    private CancellationToken _token;

    public TelegramBot(ILogger logger, CommandsExecutor executor)
    {
        _logger = logger;
        _executor = executor;

        _cts = new CancellationTokenSource();
        _token = _cts.Token;
    }

    public void Start(AppConfig config)
    {
        _cts = new CancellationTokenSource();
        _token = _cts.Token;

        Listen(config.BotConfig.ChatId, new TelegramBotApiWrapper(_currentConfig.BotConfig.ApiUri, _currentConfig.BotConfig.ApiKey), _token);

        Status = ControlPocessorEnum.Working;
        _currentConfig = config;
    }

    private async void Listen(int chatId, TelegramBotApiWrapper wrapper, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var response = await wrapper.GetUpdates();

            if(!response.Ok)
                continue;

            var messages = response.Result
                .Where(x => x.Message?.Chat?.Id == chatId && (DateTime.Now - x.Message.ParsedDate).Minutes < 5)
                .Select(x => x.Message?.Text).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var responses = _executor.Execute(messages);

            foreach (var res in responses.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                await wrapper.SendResponse(chatId, res!, _buttons);
            }

            await Task.Delay(RefreshTime, token);
        }
    }

    public void Restart(AppConfig config)
    {
        Stop();
        Status = ControlPocessorEnum.Stopped;

        Start(config);
        Status = ControlPocessorEnum.Working;
    }

    public void Restart()
    {
        Restart(_currentConfig);
    }

    public void Stop()
    {
        _cts.Cancel();
        Status = ControlPocessorEnum.Stopped;
    }
}