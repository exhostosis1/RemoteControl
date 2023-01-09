using Shared.Config;
using Shared.ControlProcessor;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer: ServerProcessor
{
    private readonly IHttpListener _listener;

    public SimpleServer(IHttpListener listener, AbstractMiddleware middleware, ILogger logger, ServerConfig? config = null): base(logger, config)
    {
        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;

        _listener.OnStatusChange += status => StatusObservers.ForEach(x => x.OnNext(status));
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
        Start(config);
    }

    public override void Stop()
    {
        _listener.StopListen();
    }
}