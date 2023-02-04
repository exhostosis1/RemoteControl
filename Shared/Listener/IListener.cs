using Shared.DataObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listener;

public interface IListener<TContext, in TParam> where TContext : IContext where TParam : StartParameters
{
    public bool IsListening { get; }
    public void StartListen(TParam param);
    public void StopListen();
    public Task<TContext> GetContextAsync(CancellationToken token = default);
    public TContext GetContext();
}