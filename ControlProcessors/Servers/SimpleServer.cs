using Shared.Config;
using Shared.ControlProcessor;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer: ServerProcessor
{
    private readonly IHttpListener _listener;
    private readonly ILogger<SimpleServer> _logger;

    public SimpleServer(IHttpListener listener, AbstractMiddleware middleware, ILogger<SimpleServer> logger, ServerConfig? config = null): base(config)
    {
        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;
        _logger = logger;

        _listener.OnStatusChange += status =>
        {
            Working = status;
            StatusObservers.ForEach(x => x.OnNext(status));
        };
    }

    protected override void StartInternal(ServerConfig config)
    {
        if (config.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(config.Uri);
    }

    protected override void RestartInternal(ServerConfig config)
    {
        Stop();
        StartInternal(config);
    }

    public override void Stop()
    {
        _listener.StopListen();
    }
}