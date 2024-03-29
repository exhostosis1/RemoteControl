using System;
using System.Collections.Generic;

namespace Shared.Observable;

public class Unsubscriber<T>(IList<IObserver<T>> observers, IObserver<T> observer) : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        observers.Remove(observer);
    }
}