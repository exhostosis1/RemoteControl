using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers;

public class SimpleServer: IControlProcessor
{
    private static readonly ServerConfig DefaultConfig = new()
    {
        Scheme = "http",
        Host = "localhost",
        Port = 80
    };

    private readonly IListener _listener;

    public CommonConfig CurrentConfig { get; private set; }
    
    public string Name { get; set; }

    public ControlProcessorType Type => ControlProcessorType.Server;

    public ControlProcessorStatus Status =>
        _listener.IsListening ? ControlProcessorStatus.Working : ControlProcessorStatus.Stopped;
    
    public string Info => _listener.ListeningUris.FirstOrDefault()?.ToString() ?? string.Empty;

    private readonly ILogger _logger;

    public SimpleServer(string name, IListener listener, AbstractMiddleware middleware, ILogger logger, ServerConfig? config = null)
    {
        Name = name;

        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;

        _logger = logger;
        CurrentConfig = config ?? DefaultConfig;
    }

    public void Start(CommonConfig? config)
    {
        var c = (ServerConfig)(config ?? CurrentConfig);

        if (c.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(c.Uri);
        CurrentConfig = c;
    }


    public void Restart(CommonConfig? config)
    {
        Stop();
        Start(config ?? CurrentConfig);
    }

    public void Stop() => _listener.StopListen();
}