using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers;

public class SimpleServer: IControlProcessor
{
    private static readonly ProcessorConfigItem DefaultConfig = new()
    {
        Scheme = "http",
        Host = "localhost",
        Port = 80
    };

    private readonly IListener _listener;

    private ProcessorConfigItem _currentConfig = DefaultConfig;
    
    public string Name { get; set; } = "webserver";

    public ControlProcessorType Type => ControlProcessorType.Server;

    public ControlProcessorStatus Status =>
        _listener.IsListening ? ControlProcessorStatus.Working : ControlProcessorStatus.Stopped;
    
    public string Info => _listener.ListeningUris.FirstOrDefault()?.ToString() ?? string.Empty;

    public SimpleServer(IListener listener, AbstractMiddleware middleware)
    {
        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;
    }

    public void Start(ProcessorConfigItem? config)
    {
        var c = config ?? _currentConfig;

        if (c.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(c.Uri);
        _currentConfig = c;
    }


    public void Restart(ProcessorConfigItem? config)
    {
        Stop();
        Start(config ?? _currentConfig);
    }

    public void Stop() => _listener.StopListen();
}