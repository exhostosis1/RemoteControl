using Shared.Config;
using Shared.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Shared.ControlProcessor;

public abstract class GenericControlProcessor<T> : AbstractControlProcessor, IObservable<T> where T : CommonConfig, new()
{
    private T _currentConfig;

    public T CurrentConfig
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            _observers.ForEach(x => x.OnNext(value));
        }
    }

    public static T DefaultConfig { get; protected set; } = new();

    private readonly List<IObserver<T>> _observers = new();

    public override void Start(CommonConfig? config = null) => Start(config as T ?? CurrentConfig);
    public override void Restart(CommonConfig? config = null) => Restart(config as T ?? CurrentConfig);
    public void Start(T? config = null)
    {
        if (config != null)
            CurrentConfig = config;

        StartInternal(CurrentConfig);
    }
    public void Restart(T? config = null)
    {
        if (config != null)
            CurrentConfig = config;

        RestartInternal(CurrentConfig);
    }

    protected abstract void StartInternal(T config);
    protected abstract void RestartInternal(T config);

    protected GenericControlProcessor(ILogger logger, T? config = null) : base(logger)
    {
        _currentConfig = config ?? DefaultConfig;
    }

    protected override CommonConfig GetCurrentConfig() => CurrentConfig;
    protected override void SetCurrentConfig(CommonConfig config) => CurrentConfig = config as T ?? CurrentConfig;

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber<T>(_observers, observer);
    }
}