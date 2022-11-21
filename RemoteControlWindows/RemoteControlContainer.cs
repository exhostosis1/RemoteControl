using Autostart;
using ConfigProviders;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlWinForms;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Controllers;
using Shared.Logging.Interfaces;
using Web.Controllers;

namespace RemoteControlWindows
{
    public class RemoteControlContainer : IContainer
    {
        public IServer Server { get; }
        public IConfigProvider ConfigProvider { get; }
        public IAutostartService AutostartService { get; }
        public ILogger DefaultLogger { get; }
        public IUserInterface UserInterface { get; set; }

        public RemoteControlContainer()
        {
#if DEBUG
            DefaultLogger = new TraceLogger();
#else
            DefaultLogger = new FileLogger("error.log");
#endif
            var user32Wrapper = new User32Provider(DefaultLogger);
            
            var controllers = new BaseController[]
            {
                new AudioController(new NAudioProvider(DefaultLogger), DefaultLogger),
                new DisplayController(user32Wrapper, DefaultLogger),
                new KeyboardController(user32Wrapper, DefaultLogger),
                new MouseController(user32Wrapper, DefaultLogger)
            };

            var endPoint = new ApiMiddlewareV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            var listener = new GenericListener(DefaultLogger);

            Server = new SimpleServer(listener, staticMiddleware);
            ConfigProvider = new LocalFileConfigProvider(DefaultLogger);
            AutostartService = new WinRegistryAutostartService();
            UserInterface = new WinFormsUI();
        }
    }
}