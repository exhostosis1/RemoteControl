using Shared.Config;

namespace Shared.ControlProcessor;

public interface IControlProcessor
{
    public string Name { get; set; }
    public bool Working { get; }
    public CommonConfig CurrentConfig { get; set; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
}