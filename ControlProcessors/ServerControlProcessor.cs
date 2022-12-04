using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Server.Interfaces;

namespace ControlProcessors;

public class ServerControlProcessor: BaseControlProcessor
{
    private readonly IServer _server;

    public override ControlPocessorEnum Status => _server.IsListening ? ControlPocessorEnum.Working : ControlPocessorEnum.Stopped;

    public ServerControlProcessor(string name, IServer server, ILogger logger) : base(name, logger, ControlProcessorType.Server)
    {
        _server = server;
    }

    protected override void StartInternal(AppConfig config)
    {
        try
        {
            _server.Start(config.ServerConfig.Uri);
            
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
    }
}