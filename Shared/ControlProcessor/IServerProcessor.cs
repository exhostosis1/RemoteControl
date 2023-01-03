using Shared.Config;

namespace Shared.ControlProcessor;

public interface IServerProcessor : IControlProcessor
{
    new ServerConfig CurrentConfig { get; set; }
    public void Start(ServerConfig? config = null);
    public void Restart(ServerConfig? config = null);
}