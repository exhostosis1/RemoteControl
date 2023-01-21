using Shared.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listeners;

public record StartParameters(string Uri, string? ApiKey = null, List<string>? Usernames = null);

public class ListenerState : IObservable<bool>
{
    private readonly List<IObserver<bool>> _observers = new();

    private bool _listening;

    public bool Listening
    {
        get => _listening;
        set
        {
            _listening = value;
            _observers.ForEach(x => x.OnNext(value));
        }
    }

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber<bool>(_observers, observer);
    }
}

public interface IListener<T> where T: IContext
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<T> GetContextAsync(CancellationToken token = default);
    public T GetContext();
}