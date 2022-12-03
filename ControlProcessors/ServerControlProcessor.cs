using Shared.Config;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace ControlProcessors;

public class ServerControlProcessor: BaseControlProcessor
{
    private readonly IServer _server;

    public ServerControlProcessor(string name, IServer server, ControlFacade facade, AppConfig config, ILogger logger) : base(name,
        facade, config, logger, ControlProcessorType.Server)
    {
        _server = server;
    }

    protected override void StartInternal(AppConfig config)
    {
        try
        {
            _server.Start(config.ServerConfig.Uri);
            Status = ControlPocessorEnum.Working;
            Info = _server.GetListeningUri()?.ToString() ?? string.Empty;
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }

    protected override void StopInternal()
    {
        _server.Stop();
        Status = ControlPocessorEnum.Stopped;
    }
}