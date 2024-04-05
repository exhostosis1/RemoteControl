using Microsoft.Extensions.Logging;
using Shared.Config;
using Shared.Listener;
using Shared.Server;
using System.ComponentModel;
using Shared.DataObjects;

namespace Servers;

public class Server: INotifyPropertyChanged
{
    public ServerType Type { get; set;  }
    public int Id { get; set; } = -1;
    public bool Status { get; private set; } = false;

    private readonly ILogger _logger;

    private readonly IListener _listener;
    private readonly Func<IContext, Task> _middleware;

    private CancellationTokenSource? _cts;
    private readonly IProgress<bool> _progress;

    private readonly TaskFactory _factory = new();

    public ServerConfig DefaultConfig { get; init; }

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

    public Server(ServerType type, IListener listener, IMiddleware[] middlewares, ILogger logger)
    {
        Type = type;
        DefaultConfig = new ServerConfig(type);
        _currentConfig = DefaultConfig;

        _logger = logger;
        _listener = listener;

        var action = (IContext context) => Task.FromException(new Exception("Should never be called"));

        for (var i = middlewares.Length - 1; i >= 0; i--)
        {
            var middleware = middlewares[i];
            var act = action;
            action = context => middleware.ProcessRequestAsync(context, act);
        }

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
                context.Response.Close();
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