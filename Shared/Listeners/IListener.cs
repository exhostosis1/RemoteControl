using Shared.DataObjects;
using System;
using System.Collections.Generic;

namespace Shared.Listeners;

public record StartParameters(Uri Uri, string? ApiKey = null, List<string>? Usernames = null);

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

public interface IListener<out T>: IObservable<T> where T: IContext
{
    public ListenerState State { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
}