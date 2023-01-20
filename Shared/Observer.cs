using System;

namespace Shared;

public class Observer<T> : IObserver<T>
{
    private readonly Action<T> _next;
    private readonly Action? _completed;
    private readonly Action<Exception>? _error;

    public Observer(Action<T> next, Action? completed = null, Action<Exception>? error = null)
    {
        _next = next;
        _completed = completed;
        _error = error;
    }

    public void OnCompleted()
    {
        _completed?.Invoke();
    }

    public void OnError(Exception error)
    {
        _error?.Invoke(error);
    }

    public void OnNext(T value)
    {
        _next(value);
    }
}