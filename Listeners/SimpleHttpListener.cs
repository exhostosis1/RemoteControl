using Microsoft.Extensions.Logging;
using Shared.DataObjects;
using Shared.Listener;
using Shared.Wrappers.HttpListener;

namespace Listeners;

public class SimpleHttpListener(IHttpListener wrapper, ILogger logger): IListener
{
    public bool IsListening => wrapper.IsListening;

    public void StartListen(StartParameters param)
    {
        if (wrapper.IsListening)
        {
            wrapper.Stop();
        }

        wrapper.GetNew();
        wrapper.Prefixes.Add(param.Uri);

        wrapper.Start();

        logger.LogInformation("Http listener started listening on {uri}", param.Uri);
    }

    public void StopListen()
    {
        if (wrapper.IsListening)
            wrapper.Stop();

        logger.LogInformation("Http listener stopped");
    }

    public async Task<IContext> GetContextAsync(CancellationToken token = default)
    {
        return await wrapper.GetContextAsync();
    }

    public IContext GetContext()
    {
        return wrapper.GetContext();
    }
}