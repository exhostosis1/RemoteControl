using Autostart;
using ConfigProviders;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlConsole;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Controllers;
using Shared.Logging.Interfaces;
using Web.Controllers;

namespace RemoteControlLinux
{
    public class RemoteControlContainer : IContainer
    {
        public IServer Server { get; }
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

            var endPoint = new ApiMiddlewareV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            var listener = new GenericListener(Logger);

            Server = new SimpleServer(listener, staticMiddleware);
            ConfigProvider = new LocalFileConfigProvider(Logger);
            AutostartService = new DummyAutostartService();
            UserInterface = new ConsoleUI();
        }
    }
}