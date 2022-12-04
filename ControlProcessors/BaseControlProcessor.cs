using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProcessors;

public abstract class BaseControlProcessor: IControlProcessor
{
    public string Name { get; set; }
    public virtual ControlPocessorEnum Status { get; protected set; } = ControlPocessorEnum.Stopped;
    public ControlProcessorType Type { get; set; }
    public string Info { get; protected set; } = string.Empty;

    protected readonly ILogger Logger;
    protected AppConfig? CurrentConfig;

    public void Start(AppConfig config)
    {
        StartInternal(config);
        CurrentConfig = config;
        Status = ControlPocessorEnum.Working;
    }

    public void Restart(AppConfig config)
    {
        Stop();
        Status = ControlPocessorEnum.Stopped;

        Start(config);
        CurrentConfig = config;
        Status = ControlPocessorEnum.Working;
    }
    public void Restart()
    {
        if (CurrentConfig == null)
            return;

        Stop();
        Status = ControlPocessorEnum.Stopped;

        Start(CurrentConfig);
        Status = ControlPocessorEnum.Working;
    }
    public void Stop()
    {
        StopInternal();
        Status = ControlPocessorEnum.Stopped;
    }

    protected BaseControlProcessor(string name, ILogger logger, ControlProcessorType type = ControlProcessorType.Common)
    {
        Logger = logger;
        Name = name;
        Type = type;
    }

    protected abstract void StartInternal(AppConfig config);
    protected abstract void StopInternal();
}