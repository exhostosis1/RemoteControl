using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers;

public class SimpleServer: ServerProcessor, IDisposable
{
    private readonly IListener<HttpContext> _listener;
    private readonly ILogger<SimpleServer> _logger;

    private readonly IDisposable _statusUnsubscriber;
    private readonly IDisposable _requestUnsubscriber;

    public SimpleServer(IListener<HttpContext> listener, AbstractMiddleware<HttpContext> middleware, ILogger<SimpleServer> logger, ServerConfig? config = null): base(config)
    {
        _logger = logger;
        _listener = listener;
        _statusUnsubscriber = _listener.Subscribe(new Observer<HttpContext>(middleware.ProcessRequest));

        _requestUnsubscriber = _listener.State.Subscribe(new Observer<bool>(status =>
        {
            Working = status;
            StatusObservers.ForEach(x => x.OnNext(status));
        }));
    }

    protected override void StartInternal(ServerConfig config)
    {
        if (config.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(new StartParameters(config.Uri));
    }

    public override void Stop()
    {
        _listener.StopListen();
    }

    public void Dispose()
    {
        _statusUnsubscriber.Dispose();
        _requestUnsubscriber.Dispose();
    }
}