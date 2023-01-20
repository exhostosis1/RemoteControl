using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net.Sockets;
using Shared;

namespace Listeners;

public class ActiveBotListener: IListener<BotContext>
{
    public ListenerState State { get; private set; }

    private List<string> _usernames = new();

    private readonly ILogger<ActiveBotListener> _logger;
    private readonly IActiveApiWrapper _wrapper;

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    private readonly List<IObserver<bool>> _statusSubscribers = new();
    private readonly List<IObserver<BotContext>> _requestSubscribers = new();

    public ActiveBotListener(IActiveApiWrapper wrapper, ILogger<ActiveBotListener> logger)
    {
        State = new ListenerState();
        _logger = logger;
        _wrapper = wrapper;
        
        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {string.Join(';', _usernames)}" : "Telegram bot stopped");
            State.Listening = result;
            _statusSubscribers.ForEach(x => x.OnNext(result));
        });
    }

    public void StartListen(StartParameters param)
    {
        if (State.Listening) return;

        if (param.Usernames != null)
            _usernames = param.Usernames;

        _cst = new CancellationTokenSource();

        _factory.StartNew(async () => await ListenAsync(param.Uri.ToString(), param.ApiKey ?? "", _cst.Token), TaskCreationOptions.LongRunning);
    }

    public void StopListen()
    {
        if (!State.Listening) return;

        try
        {
            _cst?.Cancel();
            _cst?.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private async Task ListenAsync(string apiUrl, string apiKey, CancellationToken token)
    {
        var internetMessageShown = false;

        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var response = await _wrapper.GetContextAsync(apiUrl, apiKey, _usernames, token);
                token.ThrowIfCancellationRequested();
                
                foreach (var context in response)
                {
                    try
                    {
                        _requestSubscribers.ForEach(x => x.OnNext(context));

                        if (!string.IsNullOrWhiteSpace(context.Result))
                        {
                            await _wrapper.SendResponseAsync(apiUrl, apiKey, context.Id, context.Result, token,
                                context.Buttons);
                        }
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
                _requestSubscribers.ForEach(x => x.OnError(e));
                _logger.LogError(e.Message);
                break;
            }
        }

        _requestSubscribers.ForEach(x => x.OnCompleted());
        _progress.Report(false);
    }

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _statusSubscribers.Add(observer);
        return new Unsubscriber<bool>(_statusSubscribers, observer);
    }

    public IDisposable Subscribe(IObserver<BotContext> observer)
    {
        _requestSubscribers.Add(observer);
        return new Unsubscriber<BotContext>(_requestSubscribers, observer);
    }
}