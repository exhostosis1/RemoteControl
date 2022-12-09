using Shared.Config;
using Shared.Enums;

namespace Shared;

public interface IControlProcessor
{
    public string Name { get; set; }
    public ControlProcessorType Type { get; }
    public ControlProcessorStatus Status { get; }
    public string Info { get; }
    public void Start(ProcessorConfigItem? config = null);
    public void Restart(ProcessorConfigItem? config = null);
    public void Stop();
}