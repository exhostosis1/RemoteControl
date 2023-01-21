using System;
using System.Collections.Generic;
using Shared.Config;

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

public interface IServer
{
    public int Id { get; }
    public ServerStatus Status { get; }
    public CommonConfig Config { get; set; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
}

public interface IServer<TConfig>: IServer, IObservable<TConfig> where TConfig: CommonConfig, new()
{
    public void Start(TConfig? config = null);
    public void Restart(TConfig? config = null);

    public TConfig DefaultConfig { get; }
    public TConfig CurrentConfig { get; set; }
}