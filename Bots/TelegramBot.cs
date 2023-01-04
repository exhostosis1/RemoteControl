using Bots.Telegram;
using Shared.Config;
using Shared.Controllers;
using Shared.ControlProcessor;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: IBotProcessor
{
    private readonly ILogger _logger;

    private static readonly BotConfig DefaultConfig = new()
    {
        ApiKey = "",
        ApiUri = "https://api.telegram.org/bot"
    };

    public bool Working { get; private set; }

    public string Name { get; set; }

    private const int RefreshTime = 1_000;

    public BotConfig CurrentConfig { get; set; }

    CommonConfig IControlProcessor.CurrentConfig
    {
        get => CurrentConfig;
        set => CurrentConfig = value as BotConfig ?? DefaultConfig;
    }

    private readonly ICommandExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    private CancellationTokenSource? _cts;

    private readonly Progress<bool> _progress;

    public TelegramBot(string name, ICommandExecutor executor, ILogger logger, BotConfig? config = null)
    {
        _logger = logger;
        _executor = executor;
        CurrentConfig = config ?? DefaultConfig;
        Name = name;

        _progress = new Progress<bool>(result => Working = result);
    }

    public void Start(BotConfig? config)
    {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var c = config ?? CurrentConfig;

        if(string.IsNullOrWhiteSpace(c.ApiUri) || string.IsNullOrWhiteSpace(c.ApiKey))
        {
            _logger.LogError("Wrong bot api configuration");
            return;
        }

        CurrentConfig = c;

#pragma warning disable CS4014
        Listen(c.Usernames, new TelegramBotApiWrapper(c.ApiUri, c.ApiKey), _progress, token);
#pragma warning restore CS4014
    }

    private async Task Listen(ICollection<string> usernames, TelegramBotApiWrapper wrapper, IProgress<bool> progress, CancellationToken token)
    {
        _logger.LogInfo($"Telegram Bot starts responding to {string.Join(',', usernames)}");
        progress.Report(true);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var response = await wrapper.GetUpdates();

                if (!response.Ok || response.Result.Length == 0)
                    continue;

                var messages = response.Result
                    .Where(x => usernames.Any(y => y == x.Message?.From?.Username) &&
                                (DateTime.Now - x.Message?.ParsedDate)?.Seconds < 10)
                    .Select(x => (x.Message?.Chat?.Id, x.Message?.Text))
                    .Where(x => x.Id.HasValue && !string.IsNullOrWhiteSpace(x.Text));

                foreach (var (id, command) in messages)
                {
                    try
                    {
                        var result = _executor.Execute(command!);
                        await wrapper.SendResponse(id!.Value, result, _buttons);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                continue;
            }
            finally
            {
                try
                {
                    await Task.Delay(RefreshTime, token);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        _logger.LogInfo("Telegram Bot stopped");
        progress.Report(false);
    }

    public void Restart(BotConfig? config)
    {
        Stop();
        Start(config ?? CurrentConfig);
    }

    public void Start(CommonConfig? config) => Start(config as BotConfig ?? CurrentConfig);
    public void Restart(CommonConfig? config) => Restart(config as BotConfig ?? CurrentConfig);

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