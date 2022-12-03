using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProcessors;

public abstract class BaseControlProcessor: IControlProcessor
{
    public void Start(AppConfig config)
    {
        StartInternal(config);
    }
    public void Restart()
    {
        Stop();
        Start(CurrentConfig);
    }
    public void Stop()
    {
        StopInternal();
    }

    public ControlPocessorEnum Status { get; protected set; } = ControlPocessorEnum.Stopped;

    public ControlFacade ControlFacade { get; }

    public string Name { get; set; }

    public AppConfig CurrentConfig { get; set; }

    public ControlProcessorType Type { get; set; }
    public string Info { get; protected set; } = string.Empty;

    protected readonly ILogger Logger;

    protected BaseControlProcessor(string name, ControlFacade controlFacade, AppConfig config, ILogger logger, ControlProcessorType type = ControlProcessorType.Common)
    {
        Logger = logger;
        CurrentConfig = config;
        ControlFacade = controlFacade;
        Name = name;
    }

    protected abstract void StartInternal(AppConfig config);
    protected abstract void StopInternal();
}