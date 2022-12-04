using Autostart;
using ConfigProviders;
using Controllers;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlConsole;
using Servers;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace RemoteControlLinux
{
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
            var ydotoolWrapper = new YdotoolProvider(Logger);
            var dummyWrapper = new DummyProvider(Logger);

            var controllers = new BaseController[]
            {
                new AudioController(dummyWrapper, Logger),
                new DisplayController(dummyWrapper, Logger),
                new KeyboardController(ydotoolWrapper, Logger),
                new MouseController(ydotoolWrapper, Logger)
            };

            var apiEndpoint = new ApiV1Endpoint(controllers, Logger);
            var staticEndpoint = new StaticFilesEndpoint(Logger);
            var router = new RoutingMiddleware(new AbstractEndpoint[] { apiEndpoint, staticEndpoint }, Logger);

            var listener = new GenericListener(Logger);

            var server = new SimpleServer(listener, router);
            ConfigProvider = new LocalFileConfigProvider(Logger);
            AutostartService = new DummyAutostartService();
            UserInterface = new ConsoleUI();

            ControlProcessors.Add(server);
        }
    }
}