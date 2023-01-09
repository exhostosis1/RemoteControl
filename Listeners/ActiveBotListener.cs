using Shared;
using Shared.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net.Sockets;
using Shared.ApiControllers;

namespace Listeners;

public class ActiveBotListener: IBotListener
{
    public bool IsListening { get; private set; }

    public List<string> Usernames { get; private set; } = new();
    public event BoolEventHandler? OnStatusChange;

    private readonly ILogger _logger;

    private readonly IApiWrapper _wrapper;
    private readonly ICommandExecutor _executor;

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;
    private const int RefreshTime = 1000;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    public ActiveBotListener(IApiWrapper wrapper, ICommandExecutor executor, ILogger logger)
    {
        _logger = logger;
        _wrapper = wrapper;
        _executor = executor;
        
        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {string.Join(';', Usernames)}" : "Telegram bot stopped");
            IsListening = result;
            OnStatusChange?.Invoke(result);
        });
    }

    public void StartListen(string apiUri, string apiKey, List<string> usernames)
    {
        if (IsListening) return;

        Usernames = usernames;

        _cst = new CancellationTokenSource();

#pragma warning disable CS4014
        Listen(apiUri, apiKey, _cst.Token);
#pragma warning restore CS4014
    }

    public void StopListen()
    {
        if (!IsListening) return;

        try
        {
            _cst?.Cancel();
            _cst?.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private async Task Listen(string apiUrl, string apiKey, CancellationToken token)
    {
        var internetMessageShown = false;

        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var response = await _wrapper.GetUpdates(apiUrl, apiKey, token);
                token.ThrowIfCancellationRequested();

                internetMessageShown = false;

                if (!response.Ok || response.Result.Length == 0)
                    continue;

                var messages = response.Result
                    .Where(x =>
                        Usernames.Any(y => y == x.Message?.From?.Username) &&
                        (DateTime.Now - x.Message?.ParsedDate)?.Seconds < 10 &&
                        x.Message?.Chat?.Id != null &&
                        !string.IsNullOrWhiteSpace(x.Message?.Text))
                    .Select(x => (x.Message!.Chat!.Id, x.Message.Text!));

                foreach (var (id, command) in messages)
                {
                    try
                    {
                        var result = _executor.Execute(command);
                        await _wrapper.SendResponse(apiUrl, apiKey, id, result, token, _buttons);
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

        _progress.Report(false);
    }
}