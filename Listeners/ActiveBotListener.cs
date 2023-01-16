using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net.Sockets;

namespace Listeners;

public class ActiveBotListener: IBotListener
{
    public bool IsListening { get; private set; }

    private List<string> _usernames = new();

    public event EventHandler<bool>? OnStatusChange;
    public event EventHandler<BotContext>? OnRequest;

    private readonly ILogger<ActiveBotListener> _logger;
    private readonly IActiveApiWrapper _wrapper;

    private CancellationTokenSource? _cst;
    private readonly IProgress<bool> _progress;

    private const int Delay = 1_000;
    private readonly TaskFactory _factory = new();

    public ActiveBotListener(IActiveApiWrapper wrapper, ILogger<ActiveBotListener> logger)
    {
        _logger = logger;
        _wrapper = wrapper;
        
        _progress = new Progress<bool>(result =>
        {
            _logger.LogInfo(result ? $"Telegram Bot starts responding to {string.Join(';', _usernames)}" : "Telegram bot stopped");
            IsListening = result;
            OnStatusChange?.Invoke(this, result);
        });
    }

    public void StartListen(string apiUri, string apiKey, List<string> usernames)
    {
        if (IsListening) return;

        _usernames = usernames;

        _cst = new CancellationTokenSource();

        _factory.StartNew(async () => await ListenAsync(apiUri, apiKey, _cst.Token), TaskCreationOptions.LongRunning);
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
                        OnRequest?.Invoke(this, context);

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
                _logger.LogError(e.Message);
                break;
            }
        }

        _progress.Report(false);
    }
}