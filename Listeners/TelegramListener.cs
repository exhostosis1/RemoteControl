using Shared.Bots.Telegram;
using Shared.DataObjects.Bot;
using Shared.Logging.Interfaces;
using System.Net.Sockets;
using Shared.Observable;
using Shared.Listener;

namespace Listeners;

public class TelegramListener : IBotListener
{
    private class LocalResponse(string apiUrl, string apiKey, int chatId, IBotApiProvider wrapper) : BotContextResponse
    {
        private readonly string _apiKey = apiKey;
        private readonly string _apiUrl = apiUrl;
        private readonly int _chatId = chatId;
        private readonly IBotApiProvider _wrapper = wrapper;

        public override void Close()
        {
            _wrapper.SendResponse(_apiUrl, _apiKey, _chatId, Message, Buttons);
        }
    }

    public bool IsListening { get; private set; }
    private List<string> _usernames = [];

    private readonly ILogger<TelegramListener> _logger;
    private readonly IBotApiProvider _wrapper;

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly Queue<BotContextRequest> _updates = new();
    private readonly SemaphoreSlim _semaphore = new(0);

    private readonly List<IObserver<bool>> _statusObservers = new();

    public TelegramListener(IBotApiProvider wrapper, ILogger<TelegramListener> logger)
    {
        _logger = logger;
        _wrapper = wrapper;

        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {string.Join(';', _usernames)}" : "Telegram bot stopped");
            IsListening = result;
            _statusObservers.ForEach(x => x.OnNext(result));
        });
    }

    public void StartListen(BotParameters param)
    {
        if (IsListening) return;
            
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
                        !string.IsNullOrWhiteSpace(update.Message.Text))
                    {
                        _updates.Enqueue(new BotContextRequest(apiUrl, apiKey, update.Message.Chat.Id, update.Message.Text, update.Message.ParsedDate));
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
                    await _logger.LogErrorAsync("Internet seems off");
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
                await _logger.LogErrorAsync(e.Message);
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

    private BotContext CreateContext(BotContextRequest request) => new(request, new LocalResponse(request.ApiUrl, request.ApiKey, request.Id, _wrapper));

    public async Task<BotContext> GetContextAsync(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(token);

            var request = _updates.Dequeue();
            if((DateTime.Now - request.Date).Seconds > 15)
                continue;

            return CreateContext(request);
        }

        throw new OperationCanceledException();
    }

    public BotContext GetContext()
    {
        while (IsListening)
        {
            _semaphore.Wait();

            var request = _updates.Dequeue();
            if ((DateTime.Now - request.Date).Seconds > 15)
                continue;

            return CreateContext(request);
        }

        throw new OperationCanceledException();
    }

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _statusObservers.Add(observer);
        return new Unsubscriber<bool>(_statusObservers, observer);
    }
}