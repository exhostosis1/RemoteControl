using Shared.Bots.Telegram;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net.Sockets;

namespace Listeners;

public class TelegramListener: IListener<BotContext>
{
    private class LocalResponse: BotContextResponse
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly int _chatId;
        private readonly IBotApiProvider _wrapper;

        public LocalResponse(string apiUrl, string apiKey, int chatId, IBotApiProvider wrapper)
        {
            _apiKey = apiKey;
            _apiUrl = apiUrl;
            _chatId = chatId;

            _wrapper = wrapper;
        }

        public override void Close()
        {
            _wrapper.SendResponse(_apiUrl, _apiKey, _chatId, Message, Buttons);
        }
    }

    public bool IsListening { get; private set; }
    private List<string> _usernames = new();

    private readonly ILogger<TelegramListener> _logger;
    private readonly IBotApiProvider _wrapper;

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly Queue<BotContextRequest> _updates = new();
    private readonly SemaphoreSlim _semaphore = new(0);

    public TelegramListener(IBotApiProvider wrapper, ILogger<TelegramListener> logger)
    {
        _logger = logger;
        _wrapper = wrapper;
        
        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {string.Join(';', _usernames)}" : "Telegram bot stopped");
            IsListening = result;
        });
    }

    public void StartListen(StartParameters param)
    {
        if (IsListening) return;

        if (param.Usernames != null)
            _usernames = param.Usernames;

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
                var updates = await _wrapper.GetUpdatesAsync(apiUrl, apiKey, token);
                token.ThrowIfCancellationRequested();

                if (!updates.Ok || updates.Result.Length == 0)
                {
                    await Task.Delay(Delay, token);
                    continue;
                }

                foreach (var update in updates.Result)
                {
                    if (_usernames.Exists(x => x == update.Message?.From?.Username) &&
                        update.Message?.Chat?.Id != null &&
                        !string.IsNullOrWhiteSpace(update.Message.Text) &&
                        (DateTime.Now - update.Message.ParsedDate).Seconds < 15)
                    {
                        _updates.Enqueue(new BotContextRequest(apiUrl, apiKey, update.Message.Chat.Id, update.Message.Text));
                        _semaphore.Release();
                    }
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
                _logger.LogError(e.Message);
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
    }

    private BotContext CreateContext(BotContextRequest request) => new (request, new LocalResponse(request.ApiUrl, request.ApiKey, request.Id, _wrapper));

    public async Task<BotContext> GetContextAsync(CancellationToken token = default)
    {
        await _semaphore.WaitAsync(token);
        return CreateContext(_updates.Dequeue());
    }

    public BotContext GetContext()
    {
        _semaphore.Wait();
        return CreateContext(_updates.Dequeue());
    }
}