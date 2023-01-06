using Bots.Telegram;
using Shared.Config;
using Shared.Controllers;
using Shared.ControlProcessor;
using Shared.Logging.Interfaces;
using System.Net.Sockets;
using Shared.ApiControllers;

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

    private const int RefreshTime = 1_000;

    public BotConfig CurrentConfig { get; set; }

    CommonConfig IControlProcessor.CurrentConfig
    {
        get => CurrentConfig;
        set => CurrentConfig = value as BotConfig ?? CurrentConfig;
    }

    private readonly ICommandExecutor _executor;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    private CancellationTokenSource? _cts;

    private readonly Progress<bool> _progress;

    public TelegramBot(ICommandExecutor executor, ILogger logger, BotConfig? config = null)
    {
        CurrentConfig = config ?? DefaultConfig;

        _logger = logger;
        _executor = executor;

        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {CurrentConfig.UsernamesString}" : "Telegram bot stopped");
            Working = result;
        });
    }

    public void Start(BotConfig? config)
    {
        if (Working) Stop();

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

    private async Task Listen(ICollection<string> usernames, TelegramBotApiWrapper wrapper, IProgress<bool> progress,
        CancellationToken token)
    {
        progress.Report(true);
        var internetMessageShown = false;

        while (!token.IsCancellationRequested)
        {
            try
            {
                var response = await wrapper.GetUpdates(token);
                token.ThrowIfCancellationRequested();

                internetMessageShown = false;

                if (!response.Ok || response.Result.Length == 0)
                    continue;

                var messages = response.Result
                    .Where(x =>
                        usernames.Any(y => y == x.Message?.From?.Username) &&
                        (DateTime.Now - x.Message?.ParsedDate)?.Seconds < 10 &&
                        x.Message?.Chat?.Id != null &&
                        !string.IsNullOrWhiteSpace(x.Message?.Text))
                    .Select(x => (x.Message!.Chat!.Id, x.Message.Text!));

                foreach (var (id, command) in messages)
                {
                    try
                    {
                        var result = _executor.Execute(command);
                        await wrapper.SendResponse(id, result, token, _buttons);
                    }
                    catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
                break;
            }
            catch (Exception e) when (e is TimeoutException || e.InnerException is SocketException)
            {
                if (!internetMessageShown)
                {
                    _logger.LogError("Internet seems off");
                    internetMessageShown = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            try
            {
                await Task.Delay(RefreshTime, token);
            }
            catch (TaskCanceledException)
            {
                break;
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
        if (!Working) return;

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