using System;
using System.Collections.Generic;

namespace Shared.Observable;

public class Unsubscriber<T>(IList<IObserver<T>> observers, IObserver<T> observer) : IDisposable
{
    private readonly IList<IObserver<T>> _observers = observers;
    private readonly IObserver<T> _observer = observer;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _observers.Remove(_observer);
    }
}