using System;

namespace Shared.Observable;

public class MyObserver<T>(Action<T> next, Action? completed = null, Action<Exception>? error = null) : IObserver<T>
{
    public void OnCompleted()
    {
        completed?.Invoke();
    }

    public void OnError(Exception error1)
    {
        error?.Invoke(error1);
    }

    public void OnNext(T value)
    {
        next(value);
    }
}