using Bots.Telegram;
using Bots.Telegram.ApiObjects.Response;
using Shared.Config;
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

    private readonly CommandsExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    private CancellationTokenSource? _cts;

    private Task? _task;

    private Progress<bool> _progress;

    public TelegramBot(string name, CommandsExecutor executor, ILogger logger, BotConfig? config = null)
    {
        _logger = logger;
        _executor = executor;
        CurrentConfig = config ?? DefaultConfig;
        Name = name;

        _progress = new Progress<bool>(result => Working = result);
    }

    public async void Start(BotConfig? config)
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

        Listen(c.Usernames, new TelegramBotApiWrapper(c.ApiUri, c.ApiKey), _progress, token);
    }

    private async Task Listen(ICollection<string> usernames, TelegramBotApiWrapper wrapper, IProgress<bool> progress, CancellationToken token)
    {
        progress.Report(true);

        while (!token.IsCancellationRequested)
        {
            UpdateResponse response;

            try
            {
                response = await wrapper.GetUpdates();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                continue;
            }

            if(!response.Ok)
                continue;

            var messages = response.Result
                .Where(x => usernames.Any(y => y == x.Message?.From?.Username) && (DateTime.Now - x.Message?.ParsedDate)?.Seconds < 10)
                .Select(x => (x.Message?.Chat?.Id, x.Message?.Text)).Where(x => x.Id.HasValue && !string.IsNullOrWhiteSpace(x.Text));

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

            try
            {
                await Task.Delay(RefreshTime, token);
            }
            catch
            {
                // catching and ignoring cancellation exception
            }
        }

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