using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Listeners;

internal interface IListener
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<RequestContext> GetContextAsync(CancellationToken token = default);
    public void CloseContext(RequestContext context);
}