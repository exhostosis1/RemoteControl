using Shared.Config;
using Shared.ControlProcessor;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Bots;

public class TelegramBot: BotProcessor
{
    private readonly IListener<BotContext> _listener;
    private readonly ILogger<TelegramBot> _logger;
    private readonly AbstractMiddleware<BotContext> _middleware;

    private readonly TaskFactory _factory = new();

    private readonly IProgress<bool> _progress;

    private CancellationTokenSource? _cts;

    public TelegramBot(IListener<BotContext> listener, AbstractMiddleware<BotContext> executor, ILogger<TelegramBot> logger, BotConfig? config = null) : base(config)
    {
        _listener = listener;
        _logger = logger;
        _middleware = executor;

        _progress = new Progress<bool>(status =>
        {
            StatusObservers.ForEach(x => x.OnNext(status));
            Working = status;
        });
    }

    protected override void StartInternal(BotConfig config)
    {
        if (Working) Stop();

        if(string.IsNullOrWhiteSpace(CurrentConfig.ApiUri) || string.IsNullOrWhiteSpace(CurrentConfig.ApiKey))
        {
            _logger.LogError("Wrong bot api configuration");
            return;
        }

        _listener.StartListen(new StartParameters(CurrentConfig.ApiUri, CurrentConfig.ApiKey, config.Usernames));
        _cts = new CancellationTokenSource();

        _factory.StartNew(async () => await Listen(_cts.Token), TaskCreationOptions.LongRunning);
    }

    private async Task Listen(CancellationToken token = default)
    {
        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync(token);
                token.ThrowIfCancellationRequested();

                _middleware.ProcessRequest(context);

                context.Response.Close();
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
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

    public override void Stop()
    {
        if (!Working) return;

        _listener.StopListen();
        
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch(ObjectDisposedException){}
    }
}