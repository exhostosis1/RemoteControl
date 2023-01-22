using Shared.Config;

namespace Shared.Server;

public interface IServer
{
    public int Id { get; }
    public ServerStatus Status { get; }
    public CommonConfig Config { get; set; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
}