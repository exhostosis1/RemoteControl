using Bots.Telegram;
using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: IControlProcessor
{
    private readonly ILogger _logger;

    private static readonly ProcessorConfigItem DefaultConfig = new()
    {
        ApiKey = "",
        ApiUri = "https://api.telegram.org/bot"
    };

    public ControlProcessorStatus Status => _task?.Status == TaskStatus.Running
        ? ControlProcessorStatus.Working
        : ControlProcessorStatus.Stopped;

    public string Name { get; set; } = "telegram bot";

    public ControlProcessorType Type => ControlProcessorType.Bot;

    public string Info => string.Join(';', _currentConfig?.Usernames ?? Array.Empty<string>());

    private const int RefreshTime = 1_000;

    private ProcessorConfigItem _currentConfig = DefaultConfig;

    private readonly CommandsExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Mute, Buttons.VolumeUp }
    };

    private CancellationTokenSource? _cts;

    private Task? _task;

    public TelegramBot(ILogger logger, CommandsExecutor executor)
    {
        _logger = logger;
        _executor = executor;
    }

    public void Start(ProcessorConfigItem? config)
    {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var c = config ?? _currentConfig;

        if(string.IsNullOrWhiteSpace(c.ApiUri) || string.IsNullOrWhiteSpace(c.ApiKey))
        {
            _logger.LogError("Wrong bot api configuration");
            return;
        }

        _task = Listen(c.Usernames, new TelegramBotApiWrapper(c.ApiUri, c.ApiKey), token);

        _currentConfig = c;
    }

    private async Task Listen(ICollection<string> usernames, TelegramBotApiWrapper wrapper, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var response = await wrapper.GetUpdates();

            if(!response.Ok)
                continue;

            var messages = response.Result
                .Where(x => usernames.Any(y => y == x.Message?.From?.Username) && (DateTime.Now - x.Message?.ParsedDate)?.Minutes < 5)
                .Select(x => (x.Message?.Chat?.Id, x.Message?.Text)).Where(x => x.Id.HasValue && !string.IsNullOrWhiteSpace(x.Text));

            foreach (var (id, command) in messages)
            {
                _executor.Execute(command!);
                await wrapper.SendResponse(id!.Value, "", _buttons);
            }

            try
            {
                await Task.Delay(RefreshTime, token);
            }
            catch
            {
                // catching and ignoring cancellation exception
            }
        }
    }

    public void Restart(ProcessorConfigItem? config)
    {
        Stop();
        Start(config ?? _currentConfig);
    }

    public void Stop()
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}