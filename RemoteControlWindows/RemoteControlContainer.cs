using Autostart;
using ConfigProviders;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlWinForms;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.Logging.Interfaces;
using Shared.Server;
using Web.Controllers;

namespace RemoteControlWindows
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
            var user32Wrapper = new User32Provider(Logger);
            
            var controllers = new BaseController[]
            {
                new AudioController(new NAudioProvider(Logger), Logger),
                new DisplayController(user32Wrapper, Logger),
                new KeyboardController(user32Wrapper, Logger),
                new MouseController(user32Wrapper, Logger)
            };

            var endPoint = new ApiMiddlewareV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            var listener = new GenericListener(Logger);

            Server = new SimpleServer(listener, staticMiddleware);
            ConfigProvider = new LocalFileConfigProvider(Logger);
            AutostartService = new WinRegistryAutostartService();
            UserInterface = new WinFormsUI();
        }
    }
}