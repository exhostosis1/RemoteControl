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
        public IConfigProvider Config { get; }
        public IAutostartService Autostart { get; }
        public ILogger DefaultLogger { get; }
        public IUserInterface UserInterface { get; set; }

        public RemoteControlContainer()
        {
#if DEBUG
            DefaultLogger = new TraceLogger();
#else
            DefaultLogger = new FileLogger("error.log");
#endif
            var ydotoolWrapper = new YdotoolProvider(DefaultLogger);
            var dummyWrapper = new DummyProvider(DefaultLogger);

            var controllers = new BaseController[]
            {
                new AudioController(dummyWrapper, DefaultLogger),
                new DisplayController(dummyWrapper, DefaultLogger),
                new KeyboardController(ydotoolWrapper, DefaultLogger),
                new MouseController(ydotoolWrapper, DefaultLogger)
            };

            var endPoint = new ApiEndpointV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            var listener = new GenericListener(DefaultLogger);

            Server = new SimpleServer(listener, staticMiddleware);
            Config = new LocalFileConfigProvider(DefaultLogger);
            Autostart = new DummyAutostartService();
            UserInterface = new ConsoleUI();
        }
    }
}