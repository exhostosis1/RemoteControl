using Shared.DataObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listener;

public interface IListener
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<IContext> GetContextAsync(CancellationToken toke = default);
    public IContext GetContext();
}