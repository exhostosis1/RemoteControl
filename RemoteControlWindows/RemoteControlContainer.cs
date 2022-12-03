using Autostart;
using ConfigProviders;
using ControlProcessors;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlWinForms;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Web.Controllers;

namespace RemoteControlWindows
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
            var user32Wrapper = new User32Provider(Logger);
            var audioProvider = new NAudioProvider(Logger);

            var controllers = new BaseController[]
            {
                new AudioController(audioProvider, Logger),
                new DisplayController(user32Wrapper, Logger),
                new KeyboardController(user32Wrapper, Logger),
                new MouseController(user32Wrapper, Logger)
            };

            var endPoint = new ApiMiddlewareV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            var listener = new GenericListener(Logger);

            var server = new SimpleServer(listener, staticMiddleware);

            ConfigProvider = new LocalFileConfigProvider(Logger);
            AutostartService = new WinRegistryAutostartService();
            UserInterface = new WinFormsUI();

            var facade = new ControlFacade
            {
                AudioControlProvider = audioProvider,
                DisplayControlProvider = user32Wrapper,
                KeyboardControlProvider = user32Wrapper,
                MouseControlProvider = user32Wrapper
            };

            ControlProcessors.Add(new ServerControlProcessor("webserver", server, facade, ConfigProvider.GetConfig(), Logger));
        }
    }
}