using Shared;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Wrappers.HttpListener;

namespace Listeners;

public class SimpleHttpListener : IWebListener
{
    private readonly IHttpListener _listener;
    private readonly ILogger<SimpleHttpListener> _logger;

    public bool IsListening => _listener.IsListening;

    public SimpleHttpListener(IHttpListener listener, ILogger<SimpleHttpListener> logger)
    {
        _listener = listener;
        _logger = logger;
    }

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

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        throw new NotImplementedException();
    }
}