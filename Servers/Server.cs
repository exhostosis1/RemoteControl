using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.Listeners;
using Servers.Middleware;
using System.ComponentModel;


namespace Servers;

public class Server: INotifyPropertyChanged
{
    public ServerType Type { get; }
    public bool Status { get; private set; } = false;

    private readonly ILogger _logger;

    private readonly IListener _listener;
    private readonly RequestDelegate _middleware;

    private CancellationTokenSource? _cts;
    private readonly IProgress<bool> _progress;

    private readonly TaskFactory _factory = new();
    
    private ServerConfig _currentConfig;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ServerConfig Config
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Config)));
        }
    }

    private static Func<RequestDelegate, RequestDelegate> GetFunction(IMiddleware middleware)
        => next =>
            context => middleware.ProcessRequestAsync(context, next);

    private static IEnumerable<Func<RequestDelegate, RequestDelegate>>
        GetFunctions(IEnumerable<IMiddleware> middlewares) => middlewares.Reverse().Select(GetFunction);

    public Server(ServerType type, IListener listener, IEnumerable<IMiddleware> middlewares, ILogger logger, ServerConfig? config = null)
    {
        Type = type;

        _logger = logger;
        _listener = listener;
        _currentConfig = config ?? new ServerConfig(type);

        RequestDelegate action = (context) =>
        {
            context.Status = RequestStatus.NotFound;
            context.Reply = "";
            context.OriginalRequest = null;

            return Task.CompletedTask;
        };
        action = GetFunctions(middlewares).Aggregate(action, (current, func) => func(current));

        _middleware = action;

        _progress = new Progress<bool>(status =>
        {
            Status = status;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        });
    }

    public void Start(ServerConfig? config = null)
    {
        if (config != null)
            Config = config;

        if (Status)
        {
            Stop();
        }

        var param = Config.Type switch
        {
            ServerType.Web => new StartParameters(Config.Uri.ToString()),
            ServerType.Bot => new StartParameters(Config.ApiUri, Config.ApiKey, Config.Usernames),
            _ => throw new NotSupportedException("Config type not supported")
        };
        
        try
        {
            _listener.StartListen(param);
        }
        catch (Exception e)
        {
            _logger.LogError("{e.Message}", e.Message);
            return;
        }

        _cts = new CancellationTokenSource();

        _factory.StartNew(async () => await ProcessRequestAsync(_cts.Token), TaskCreationOptions.LongRunning);
    }

    public void Restart(ServerConfig? config = null)
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
                await _middleware(context);
                _listener.CloseContext(context);
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError("{e.Message}", e.Message);
                break;
            }
        }

        _progress.Report(false);
    }

    public void Stop()
    {
        if (!Status) return;

        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch (ObjectDisposedException) { }

        _listener.StopListen();
    }
}