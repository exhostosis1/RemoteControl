using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Wrappers.HttpListener;

namespace Listeners;

public class SimpleHttpListener(IHttpListener listener, ILogger<SimpleHttpListener> logger) : IWebListener
{
    private readonly IHttpListener _listener = listener;
    private readonly ILogger<SimpleHttpListener> _logger = logger;

    public bool IsListening => _listener.IsListening;

    public void StartListen(WebParameters param)
    {
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.GetNew();
        _listener.Prefixes.Add(param.Uri);

        _listener.Start();

        _logger.LogInfo($"Http listener started listening on {param.Uri}");
    }

    public void StopListen()
    {
        if (_listener.IsListening)
            _listener.Stop();

        _logger.LogInfo("Http listener stopped");
    }

    public async Task<WebContext> GetContextAsync(CancellationToken token = default)
    {
        return await _listener.GetContextAsync();
    }

    public WebContext GetContext()
    {
        return _listener.GetContext();
    }
}