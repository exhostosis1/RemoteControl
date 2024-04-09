using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.DataObjects.Bot;
using Servers.Listeners.Telegram;
using System.ComponentModel;
using System.Net.Sockets;

namespace Servers.Listeners;

public class TelegramListener : IListener
{
    private class LocalResponse(string apiUrl, string apiKey, int chatId, TelegramBotApiProvider apiProvider) : BotContextResponse
    {
        public override void Close()
        {
            apiProvider.SendResponse(apiUrl, apiKey, chatId, Message, Buttons);
        }
    }

    public bool IsListening { get; private set; }
    private List<string> _usernames = [];

    private readonly ILogger _logger;
    private readonly TelegramBotApiProvider _apiProvider = new();

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly Queue<BotContextRequest> _updates = new();
    private readonly SemaphoreSlim _semaphore = new(0);

    public event PropertyChangedEventHandler? PropertyChanged;

    public TelegramListener(ILogger logger)
    {
        _logger = logger;

        _progress = new Progress<bool>(result =>
        {
            _logger.LogInformation(result ? $"Telegram Bot starts responding to {string.Join(';', _usernames)}" : "Telegram bot stopped");
            IsListening = result;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
        });
    }

    public void StartListen(StartParameters param)
    {
        if (IsListening) return;

        _usernames = param.Usernames ?? [];

        _cst = new CancellationTokenSource();
        _factory.StartNew(async () => await ListenAsync(param.Uri, param.ApiKey ?? "", _cst.Token), TaskCreationOptions.LongRunning);
    }

    private async Task ListenAsync(string apiUrl, string apiKey, CancellationToken token)
    {
        var internetMessageShown = false;

        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var updates = await _apiProvider.GetUpdatesAsync(apiUrl, apiKey, token);
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

                    _updates.Enqueue(new BotContextRequest(apiUrl, apiKey, update.Message.Chat.Id, update.Message.Text, update.Message.ParsedDate));
                    _semaphore.Release();
                }

                internetMessageShown = false;

                await Task.Delay(Delay, token);
            }
            catch (Exception e) when (e is TimeoutException || e.InnerException is SocketException)
            {
                if (!internetMessageShown)
                {
                    _logger.LogError("Internet seems off");
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
                _logger.LogError("{error}", e.Message);
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

    private BotContext CreateContext(BotContextRequest request) => new(request, new LocalResponse(request.ApiUrl, request.ApiKey, request.Id, _apiProvider));

    public async Task<IContext> GetContextAsync(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(token);

            var request = _updates.Dequeue();
            if ((DateTime.Now - request.Date).Seconds > 15)
                continue;

            return CreateContext(request);
        }

        throw new OperationCanceledException();
    }
}