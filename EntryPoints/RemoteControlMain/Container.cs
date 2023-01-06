using ApiControllers;
using Bots;
using Listeners;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;
using Shared.UI;

namespace RemoteControlMain;

internal class Container : IContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; }
    public ControlFacade ControlProviders { get; }
    public ICommandExecutor CommandExecutor { get; }
    public IListener Listener { get; }
    public AbstractMiddleware Middleware { get; }

    public Container(IPlatformDependantContainer input)
    {
        ConfigProvider = input.ConfigProvider;
        AutostartService = input.AutostartService;
        Logger = input.Logger;
        UserInterface = input.UserInterface;
        ControlProviders = input.ControlProviders;

        CommandExecutor = new CommandsExecutor(ControlProviders, Logger);
        Listener = new GenericListener(Logger);

        var controllers = new BaseApiController[]
        {
            new AudioController(ControlProviders.Audio, Logger),
            new DisplayController(ControlProviders.Display, Logger),
            new KeyboardController(ControlProviders.Keyboard, Logger),
            new MouseController(ControlProviders.Mouse, Logger)
        };
        var apiEndpoint = new ApiV1Endpoint(controllers, Logger);
        var staticEndpoint = new StaticFilesEndpoint(Logger);

        Middleware = new RoutingMiddleware(new[] { apiEndpoint }, staticEndpoint, Logger);
    }
}