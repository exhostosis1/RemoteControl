using System;
using Shared.Config;

namespace Shared.Server;

public interface IServer<TConfig>: IServer, IObservable<TConfig> where TConfig: CommonConfig, new()
{
    public void Start(TConfig? config = null);
    public void Restart(TConfig? config = null);

    public TConfig DefaultConfig { get; }
    public TConfig CurrentConfig { get; set; }
}