using Shared.Config;
using Shared.ControlProviders;
using Shared.Enums;

namespace Shared;

public interface IControlProcessor
{
    public void Start(AppConfig config);
    public void Restart();
    public void Stop();

    public ControlPocessorEnum Status { get; }

    public ControlFacade ControlFacade { get; }

    public string Name { get; set; }

    public AppConfig CurrentConfig { get; set; }

    public ControlProcessorType Type { get; set; }

    public string Info { get; }
}