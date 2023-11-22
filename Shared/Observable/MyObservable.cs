using System;
using System.Collections.Generic;

namespace Shared.Observable;

public class MyObservable<T>: IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    public void Next(T value)
    {
        _observers.ForEach(x => x.OnNext(value));
    }

    public void Completed()
    {
        _observers.ForEach(x => x.OnCompleted());
    }

    public void Error(Exception error)
    {
        _observers.ForEach(x => x.OnError(error));
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);

        return new Unsubscriber<T>(_observers, observer);
    }
}