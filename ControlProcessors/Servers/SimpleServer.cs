using Shared;
using Shared.Config;
using Shared.DataObjects;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer<TContext, TConfig>: IServer<TConfig> where TContext: IContext where TConfig: CommonConfig, new()
{
    public int Id { get; init; } = -1;
    public ServerStatus Status { get; } = new();

    private readonly ILogger<SimpleServer<TContext, TConfig>> _logger;
    
    private readonly IListener<TContext> _listener;
    private readonly IMiddleware<TContext> _middleware;

    private CancellationTokenSource? _cts;
    private readonly IProgress<bool> _progress;

    private readonly TaskFactory _factory = new();

    public TConfig DefaultConfig { get; } = new();

    public CommonConfig Config
    {
        get => CurrentConfig;
        set => CurrentConfig = value as TConfig ?? CurrentConfig;
    }

    private TConfig _currentConfig;

    public TConfig CurrentConfig
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            _configObservers.ForEach(x => x.OnNext(value));
        }
    }

    private readonly List<IObserver<TConfig>> _configObservers = new();

    public SimpleServer(IListener<TContext> listener, IMiddleware<TContext> middleware, ILogger<SimpleServer<TContext, TConfig>> logger, TConfig? config = null)
    {
        _currentConfig = config ?? DefaultConfig;

        _logger = logger;
        _listener = listener;
        _middleware = middleware;

        _progress = new Progress<bool>(status => Status.Working = status);
    }

    public void Start(TConfig? config = null)
    {
        if (config != null)
            CurrentConfig = config;

        if (Status.Working)
        {
            Stop();
        }

        var param = CurrentConfig switch
        {
            ServerConfig s => new StartParameters(s.Uri.ToString()),
            BotConfig b => new StartParameters(b.ApiUri, b.ApiKey, b.Usernames),
            _ => throw new NotSupportedException("Config type not supported")
        };

        try
        {
            _listener.StartListen(param);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return;
        }

        _cts = new CancellationTokenSource();

        _factory.StartNew(async () => await ProcessRequestAsync(_cts.Token), TaskCreationOptions.LongRunning);

        _logger.LogInfo($"Started listening on {param.Uri}");
    }

    public void Restart(TConfig? config = null)
    {
        Stop();
        Start(config);
    }

    private async Task ProcessRequestAsync(CancellationToken token)
    {
        _progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync(token);
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

    public void Start(CommonConfig? config = null)
    {
        var c = config as TConfig ?? CurrentConfig;
        Start(c);
    }

    public void Restart(CommonConfig? config = null)
    {
        Stop();
        Start(config);
    }

    public void Stop()
    {
        if (!Status.Working) return;

        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch (ObjectDisposedException) { }

        _listener.StopListen();

        _logger.LogInfo("Stopped listening");
    }

    public IDisposable Subscribe(IObserver<TConfig> observer)
    {
        _configObservers.Add(observer);
        return new Unsubscriber<TConfig>(_configObservers, observer);
    }
}