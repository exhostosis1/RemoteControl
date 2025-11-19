using MainApp.Interfaces;
using MainApp.Workers.DataObjects;
using MainApp.Workers.Listeners;
using MainApp.Workers.Middleware;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace MainApp.Workers;

internal class Worker : IWorker
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool Status => _listener.IsListening;

    private readonly ILogger _logger;

    private readonly IListener _listener;
    private readonly RequestDelegate _middleware;

    private CancellationTokenSource? _cts;

    private readonly TaskFactory _factory = new();

    public WorkerConfig Config { get; set; }

    private static Func<RequestDelegate, RequestDelegate> GetFunction(IMiddleware middleware)
        => next =>
            context => middleware.ProcessRequestAsync(context, next);

    private static IEnumerable<Func<RequestDelegate, RequestDelegate>>
        GetFunctions(IEnumerable<IMiddleware> middlewares) => middlewares.Reverse().Select(GetFunction);

    internal Worker(WorkerConfig config, IListener listener, IEnumerable<IMiddleware> middlewares, ILogger logger)
    {
        _logger = logger;
        _listener = listener;
        Config = config;

        RequestDelegate action = (context) =>
        {
            context.Status = RequestStatus.NotFound;
            context.Reply = "";

            return Task.CompletedTask;
        };
        action = GetFunctions(middlewares).Aggregate(action, (current, func) => func(current));

        _middleware = action;

        _listener.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(_listener.IsListening)) return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        };
    }

    public bool Start()
    {
        if (Status)
        {
            return true;
        }

        var param = Config.Type switch
        {
            WorkerType.Web => new StartParameters(Config.Uri.ToString()),
            WorkerType.Bot => new StartParameters(Config.ApiUri, Config.ApiKey, Config.Usernames),
            _ => throw new NotSupportedException("Config type not supported")
        };

        _listener.StartListen(param);

        _cts = new CancellationTokenSource();

        _factory.StartNew(async () => await ProcessRequestAsync(_cts.Token), TaskCreationOptions.LongRunning);

        return Status;
    }

    public bool Restart()
    {
        Stop();
        Start();

        return Status;
    }

    private async Task ProcessRequestAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync(token);
                await _middleware(context);

                context.Close();
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
            {
                break;
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("{e.Message}", e.Message);
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                break;
            }
        }
    }

    public bool Stop()
    {
        if (!Status) return false;

        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch (ObjectDisposedException) { }

        _listener.StopListen();

        return Status;
    }
}