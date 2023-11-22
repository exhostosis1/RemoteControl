using System;

namespace Shared.Observable;

public class MyObserver<T>(Action<T> next, Action? completed = null, Action<Exception>? error = null) : IObserver<T>
{
    private readonly Action<T> _next = next;
    private readonly Action? _completed = completed;
    private readonly Action<Exception>? _error = error;

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