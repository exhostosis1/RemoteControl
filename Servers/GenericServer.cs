using Shared.Config;
using Shared.DataObjects;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Observable;
using Shared.Server;

namespace Servers;

public class GenericServer<TContext, TConfig, TParams> : IServer<TConfig> where TContext : IContext where TConfig : CommonConfig, new() where TParams: StartParameters
{
    public int Id { get; set; } = -1;
    public ServerStatus Status { get; } = new();

    private readonly ILogger<GenericServer<TContext, TConfig, TParams>> _logger;

    private readonly IListener<TContext, TParams> _listener;
    private readonly IMiddlewareChain<TContext> _middleware;

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

    private readonly List<IObserver<TConfig>> _configObservers = [];

    protected GenericServer(IListener<TContext, TParams> listener, IMiddlewareChain<TContext> middleware, ILogger<GenericServer<TContext, TConfig, TParams>> logger)
    {
        _currentConfig = DefaultConfig;

        _logger = logger;
        _listener = listener;
        _middleware = middleware;

        _progress = new Progress<bool>(status => Status.Working = status);
    }

    public void Start(TConfig? config)
    {
        if (config != null)
            CurrentConfig = config;

        if (Status.Working)
        {
            Stop();
        }

        var param = CurrentConfig switch
        {
            WebConfig s => new WebParameters(s.Uri.ToString()) as TParams,
            BotConfig b => new BotParameters(b.ApiUri, b.ApiKey, b.Usernames) as TParams,
            _ => throw new NotSupportedException("Config type not supported")
        };

        if (param == null) return;

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
    }

    public void Restart(TConfig? config)
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
                _middleware.ChainRequest(context);
                context.Response.Close();
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
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

    public void Start(CommonConfig? config = null)
    {
        Start(config as TConfig);
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
    }

    public IDisposable Subscribe(IObserver<TConfig> observer)
    {
        _configObservers.Add(observer);
        return new Unsubscriber<TConfig>(_configObservers, observer);
    }
}