using System.ComponentModel;

namespace Shared.Server;

public interface IServer<TConfig> : IServer where TConfig : class, new()
{
    public void Start(TConfig? config = null);
    public void Restart(TConfig? config = null);

    public TConfig DefaultConfig { get; }
    public TConfig CurrentConfig { get; set; }
}