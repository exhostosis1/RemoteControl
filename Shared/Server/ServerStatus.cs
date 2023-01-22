using System;
using System.Collections.Generic;

namespace Shared.Server;

public class ServerStatus : IObservable<bool>
{
    private readonly List<IObserver<bool>> _statusObservers = new();

    private bool _working;

    public bool Working
    {
        get => _working;
        set
        {
            _working = value;
            _statusObservers.ForEach(x => x.OnNext(value));
        }
    }

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _statusObservers.Add(observer);
        return new Unsubscriber<bool>(_statusObservers, observer);
    }
}