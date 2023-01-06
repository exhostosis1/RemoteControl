using Shared.Config;
using Shared.ControlProcessor;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers;

public class SimpleServer: IServerProcessor
{
    private static readonly ServerConfig DefaultConfig = new()
    {
        Scheme = "http",
        Host = "localhost",
        Port = 80
    };

    private readonly IListener _listener;

    public ServerConfig CurrentConfig { get; set; }

    CommonConfig IControlProcessor.CurrentConfig
    {
        get => CurrentConfig;
        set => CurrentConfig = value as ServerConfig ?? DefaultConfig;
    }

    public bool Working => _listener.IsListening;
    
    private readonly ILogger _logger;

    public SimpleServer(IListener listener, AbstractMiddleware middleware, ILogger logger, ServerConfig? config = null)
    {
        CurrentConfig = config ?? DefaultConfig;

        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;

        _logger = logger;
    }

    public void Start(ServerConfig? config)
    {
        var c = config ?? CurrentConfig;

        if (c.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(c.Uri);
        CurrentConfig = c;
    }


    public void Restart(ServerConfig? config)
    {
        Stop();
        Start(config ?? CurrentConfig);
    }

    public void Start(CommonConfig? config) => Start(config as ServerConfig ?? CurrentConfig);
    public void Restart(CommonConfig? config) => Restart(config as ServerConfig ?? CurrentConfig);

    public void Stop() => _listener.StopListen();
}