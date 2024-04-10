using Servers.DataObjects;

namespace Servers.Listeners;

public interface IListener
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<RequestContext> GetContextAsync(CancellationToken token = default);
    public void CloseContext(RequestContext context);
}