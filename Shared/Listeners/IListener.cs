using Shared.DataObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listeners;

public record StartParameters(string Uri, string? ApiKey = null, List<string>? Usernames = null);

public interface IListener<T> where T : IContext
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<T> GetContextAsync(CancellationToken token = default);
    public T GetContext();
}