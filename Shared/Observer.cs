using System;

namespace Shared;

public class Observer<T> : IObserver<T>
{
    private readonly Action<T> _next;

    public Observer(Action<T> next)
    {
        _next = next;
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(T value)
    {
        _next(value);
    }
}