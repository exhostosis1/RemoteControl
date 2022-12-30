using Shared.Config;
using Shared.Enums;

namespace Shared;

public interface IControlProcessor
{
    public string Name { get; set; }
    public ControlProcessorType Type { get; }
    public ControlProcessorStatus Status { get; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
    public CommonConfig CurrentConfig { get; }
}