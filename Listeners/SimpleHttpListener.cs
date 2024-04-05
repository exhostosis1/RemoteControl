using Microsoft.Extensions.Logging;
using Shared.DataObjects;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Wrappers.HttpListener;

namespace Listeners;

public class SimpleHttpListener(IHttpListener listener, ILogger logger) : IListener
{
    public bool IsListening => listener.IsListening;

    public void StartListen(StartParameters param)
    {
        if (listener.IsListening)
        {
            listener.Stop();
        }

        listener.GetNew();
        listener.Prefixes.Add(param.Uri);

        listener.Start();

        logger.LogInformation("Http listener started listening on {uri}", param.Uri);
    }

    public void StopListen()
    {
        if (listener.IsListening)
            listener.Stop();

        logger.LogInformation("Http listener stopped");
    }

    public async Task<IContext> GetContextAsync(CancellationToken token = default)
    {
        return await listener.GetContextAsync();
    }

    public IContext GetContext()
    {
        return listener.GetContext();
    }
}