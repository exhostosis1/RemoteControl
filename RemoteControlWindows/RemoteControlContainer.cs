using Autostart;
using Bots;
using ConfigProviders;
using Controllers;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlWinForms;
using Servers;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace RemoteControlWindows;

public class RemoteControlContainer : IContainer
{
    public ICollection<IControlProcessor> ControlProcessors { get; } = new List<IControlProcessor>();
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; set; }

    public RemoteControlContainer()
    {
#if DEBUG
        Logger = new TraceLogger();
#else
            Logger = new FileLogger("error.log");
#endif
        var user32Wrapper = new User32Provider(Logger);
        var audioProvider = new NAudioProvider(Logger);

        var controllers = new BaseController[]
        {
            new AudioController(audioProvider, Logger),
            new DisplayController(user32Wrapper, Logger),
            new KeyboardController(user32Wrapper, Logger),
            new MouseController(user32Wrapper, Logger)
        };

        var apiEndpoint = new ApiV1Endpoint(controllers, Logger);
        var staticEndpoint = new StaticFilesEndpoint(Logger);

        var router = new RoutingMiddleware(new AbstractEndpoint[] { apiEndpoint, staticEndpoint }, Logger);

        var listener = new GenericListener(Logger);

        var server = new SimpleServer(listener, router);

        ConfigProvider = new LocalFileConfigProvider(Logger);
        AutostartService = new WinRegistryAutostartService();
        UserInterface = new WinFormsUI();

        ControlProcessors.Add(server);

        var executor = new CommandsExecutor(controllers);
        var bot = new TelegramBot(Logger, executor);

        ControlProcessors.Add(bot);
    }
}