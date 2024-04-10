using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.Listeners.Telegram;
using System.ComponentModel;
using System.Net.Sockets;

namespace Servers.Listeners;

public class TelegramListener : IListener
{
    public bool IsListening { get; private set; }
    private List<string> _usernames = [];

    private readonly ILogger _logger;
    private TelegramBotApiProvider _apiProvider = new("", "");

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly Queue<InputContext> _updates = new();
    private readonly SemaphoreSlim _semaphore = new(0);

    public event PropertyChangedEventHandler? PropertyChanged;

    public TelegramListener(ILogger logger)
    {
        _logger = logger;

        _progress = new Progress<bool>(result =>
        {
            if (result)
            {
                _logger.LogInformation("Telegram Bot starts responding to {usernames}", string.Join(';', _usernames));
            }
            else
            {
                _logger.LogInformation("Telegram bot stopped");
            }
            
            IsListening = result;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
        });
    }

    public void StartListen(StartParameters param)
    {
        if (IsListening) return;

        _apiProvider = new TelegramBotApiProvider(param.Uri, param.ApiKey!);

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

                    _updates.Enqueue(new InputContext
                    {
                        Id = update.Message.Chat.Id,
                        Command = update.Message.Text,
                        Date = update.Message.ParsedDate
                    });
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

    public void CloseContext(RequestContext context)
    {
        _apiProvider.SendResponse(context.Input.Id, context.Output.Message, context.Output.Buttons);
    }
    
    public async Task<RequestContext> GetContextAsync(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(token);

            var request = _updates.Dequeue();
            if ((DateTime.Now - request.Date).Seconds > 15)
                continue;

            return new RequestContext
            {
                Input = request
            };
        }

        throw new OperationCanceledException();
    }
}