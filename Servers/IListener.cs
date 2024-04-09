using Shared.DataObjects;

namespace Servers;

public interface IListener
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<IContext> GetContextAsync(CancellationToken toke = default);
}