using MainApp.Servers.DataObjects;
using MainApp.Servers.Listeners.Telegram;
using MainApp.Servers.Listeners.Telegram.BotButtons;
using MainApp.Servers.Middleware.Enums;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net.Sockets;

namespace MainApp.Servers.Listeners;

internal record UpdateObject(int Id, string Message, DateTime Date);

internal class TelegramListener : IListener
{
    public bool IsListening { get; private set; }
    private List<string> _usernames = [];

    private readonly ILogger _logger;
    private TelegramBotApiProvider _apiProvider = new("", "");

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly Queue<UpdateObject> _updates = new();
    private readonly SemaphoreSlim _semaphore = new(0);
    public event PropertyChangedEventHandler? PropertyChanged;

    public TelegramListener(ILogger logger)
    {
        _logger = logger;

        _progress = new Progress<bool>(result =>
        {
            if (result)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Telegram Bot starts responding to {usernames}", string.Join(';', _usernames));
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Telegram bot stopped");
                }
            }

            IsListening = result;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
        });
    }

    public void StartListen(StartParameters param)
    {
        if (IsListening) return;

        _apiProvider = new TelegramBotApiProvider(param.Uri.EndsWith('/') ? param.Uri : $"{param.Uri}/", param.ApiKey!);

        _usernames = param.Usernames ?? [];

        _cst = new CancellationTokenSource();
        _factory.StartNew(async () => await ListenAsync(_cst.Token), TaskCreationOptions.LongRunning);
    }

    private async Task ListenAsync(CancellationToken token)
    {
        var internetMessageShown = false;

        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var updates = await _apiProvider.GetUpdatesAsync(token);
                token.ThrowIfCancellationRequested();

                if (!updates.Ok || updates.Result.Length == 0)
                {
                    await Task.Delay(Delay, token);
                    continue;
                }

                foreach (var update in updates.Result)
                {
                    if (!_usernames.Exists(x => x == update.Message?.From?.Username) ||
                        update.Message?.Chat?.Id == null ||
                        string.IsNullOrWhiteSpace(update.Message.Text)) continue;

                    _updates.Enqueue(new UpdateObject(update.Message.Chat.Id, update.Message.Text, update.Message.ParsedDate));
                    _semaphore.Release();
                }

                internetMessageShown = false;

                await Task.Delay(Delay, token);
            }
            catch (Exception e) when (e is TimeoutException || e.InnerException is SocketException)
            {
                if (!internetMessageShown)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Internet seems off");
                    }
                    internetMessageShown = true;
                }

                await Task.Delay(Delay, token);
            }
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("{error}", e.Message);
                }
                break;
            }
        }

        _progress.Report(false);
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

        _updates.Clear();
    }

    public async Task<RequestContext> GetContextAsync(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(token);

            var request = _updates.Dequeue();
            if ((DateTime.Now - request.Date).Seconds > 15)
                continue;

            var path = "/api/v1/";

            path += request.Message switch
            {
                BotButtons.Pause => "keyboard/pause",
                BotButtons.MediaBack => "keyboard/mediaback",
                BotButtons.MediaForth => "keyboard/mediaforth",
                BotButtons.VolumeUp => "audio/increasebyfive",
                BotButtons.VolumeDown => "audio/decreasebyfive",
                BotButtons.Darken => "display/darken",
                _ => throw new NotSupportedException(request.Message)
            };

            return new BotRequestContext(_apiProvider, request.Id)
            {
                Request = path
            };
        }

        throw new OperationCanceledException();
    }

    private class BotRequestContext(TelegramBotApiProvider provider, int id) : RequestContext
    {
        private static readonly IButtonsMarkup Buttons = new ReplyButtonsMarkup(new List<List<SingleButton>>
        {
            new()
            {
                new SingleButton(BotButtons.MediaBack),
                new SingleButton(BotButtons.Pause),
                new SingleButton(BotButtons.MediaForth)
            },
            new()
            {
                new SingleButton(BotButtons.VolumeDown),
                new SingleButton(BotButtons.Darken),
                new SingleButton(BotButtons.VolumeUp)
            }
        })
        {
            Resize = true,
            Persistent = true
        };

        public override void Close()
        {
            provider.SendResponse(id, string.IsNullOrWhiteSpace(Reply) ? "done" : Reply, Buttons);
        }
    }
}