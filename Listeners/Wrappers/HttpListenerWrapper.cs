using System.Net;
using Shared.DataObjects.Http;
using Shared.Listeners;

namespace Listeners.Wrappers;

public class HttpListenerWrapper: IHttpListenerWrapper
{
    private HttpListener _listener = new();

    public bool IsListening => _listener.IsListening;

    public void Start(Uri uri)
    {
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add(uri.ToString());

        _listener.Start();
    }

    public void Stop()
    {
        if(_listener.IsListening)
            _listener.Stop();
    }

    public async Task<Context> GetContextAsync()
    {
        var context = await _listener.GetContextAsync();

        return new Context(context);
    }
}