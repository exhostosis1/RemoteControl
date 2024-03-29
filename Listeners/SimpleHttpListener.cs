using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Wrappers.HttpListener;

namespace Listeners;

public class SimpleHttpListener(IHttpListener listener, ILogger<SimpleHttpListener> logger) : IWebListener
{
    public bool IsListening => listener.IsListening;

    public void StartListen(WebParameters param)
    {
        if (listener.IsListening)
        {
            listener.Stop();
        }

        listener.GetNew();
        listener.Prefixes.Add(param.Uri);

        listener.Start();

        logger.LogInfo($"Http listener started listening on {param.Uri}");
    }

    public void StopListen()
    {
        if (listener.IsListening)
            listener.Stop();

        logger.LogInfo("Http listener stopped");
    }

    public async Task<WebContext> GetContextAsync(CancellationToken token = default)
    {
        return await listener.GetContextAsync();
    }

    public WebContext GetContext()
    {
        return listener.GetContext();
    }
}