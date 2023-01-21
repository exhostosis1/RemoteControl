using Shared.DataObjects.Http;
using Shared.Listeners;

namespace Listeners;

public class SimpleHttpListener : IListener<HttpContext>
{
    private readonly IHttpListener _listener;

    public bool IsListening => _listener.IsListening;

    public SimpleHttpListener(IHttpListener listener)
    {
        _listener = listener;
    }

    public void StartListen(StartParameters param)
    {
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.GetNew();
        _listener.Prefixes.Add(param.Uri);

        _listener.Start();
    }

    public void StopListen()
    {
        if (_listener.IsListening)
            _listener.Stop();
    }

    public async Task<HttpContext> GetContextAsync(CancellationToken token = default)
    {
        return await _listener.GetContextAsync();
    }

    public HttpContext GetContext()
    {
        return _listener.GetContext();
    }
}