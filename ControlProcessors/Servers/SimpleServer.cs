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

    public SimpleServer(IHttpListener listener, IMiddleware middleware, ILogger<SimpleServer> logger, ServerConfig? config = null): base(config)
    {
        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;
        _logger = logger;

        _listener.OnStatusChange += (sender, status) =>
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

    public override void Stop()
    {
        _listener.StopListen();
    }
}