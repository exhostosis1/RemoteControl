using Shared.Config;
using System;
using System.Collections.Generic;

namespace Shared.ControlProcessor;

public abstract class AbstractControlProcessor: IObservable<bool>, IObservable<CommonConfig>
{
    public int Id { get; init; } = -1;
    public bool Working { get; protected set; } = false;

    public CommonConfig Config
    {
        get => GetCurrentConfig();
        set
        {
            ConfigObservers.ForEach(x => x.OnNext(value));
            SetCurrentConfig(value);
        }
    }

    protected readonly List<IObserver<bool>> StatusObservers = new();
    protected readonly List<IObserver<CommonConfig>> ConfigObservers = new();

    protected abstract CommonConfig GetCurrentConfig();
    protected abstract void SetCurrentConfig(CommonConfig config);

    public abstract void Start(CommonConfig? config = null);
    public abstract void Restart(CommonConfig? config = null);
    public abstract void Stop();

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        StatusObservers.Add(observer);
        return new Unsubscriber<bool>(StatusObservers, observer);
    }

    public IDisposable Subscribe(IObserver<CommonConfig> observer)
    {
        ConfigObservers.Add(observer);
        return new Unsubscriber<CommonConfig>(ConfigObservers, observer);
    }
}