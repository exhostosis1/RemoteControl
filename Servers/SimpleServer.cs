using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers;

public class SimpleServer: IControlProcessor
{
    private readonly IListener _listener;
    private AppConfig _currentConfig;
    public string Name { get; set; } = "webserver";
    public ControlProcessorType Type => ControlProcessorType.Server;

    public ControlPocessorEnum Status =>
        _listener.IsListening ? ControlPocessorEnum.Working : ControlPocessorEnum.Stopped;
    public string Info => _listener.ListeningUris.FirstOrDefault()?.ToString() ?? string.Empty;

    public SimpleServer(IListener listener, AbstractMiddleware middleware)
    {
        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;
    }

    public void Start(AppConfig config)
    {
        _listener.StartListen(config.ServerConfig.Uri);
        _currentConfig = config;
    }


    public void Restart(AppConfig config)
    {
        Stop();
        Start(config);
    }

    public void Restart()
    {
        Stop();
        Start(_currentConfig);
    }

    public void Stop() => _listener.StopListen();
}