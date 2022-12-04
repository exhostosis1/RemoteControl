using Shared.Config;
using Shared.Enums;

namespace Shared;

public interface IControlProcessor
{
    public string Name { get; set; }
    public ControlProcessorType Type { get; }
    public ControlPocessorEnum Status { get; }
    public string Info { get; }
    public void Start(AppConfig config);
    public void Restart(AppConfig config);
    public void Restart();
    public void Stop();
}