using Autostart;
using ConfigProviders;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlApp.Controllers;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Controllers;
using Shared.ControlProviders;
using System.Runtime.InteropServices;
using RemoteControlConsole;
using Shared.Logging.Interfaces;

namespace RemoteControl
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

            IKeyboardControlProvider keyboard;
            IMouseControlProvider mouse;
            IDisplayControlProvider display;
            IAudioControlProvider audio;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var user32Wrapper = new User32Provider(DefaultLogger);

                keyboard = user32Wrapper;
                mouse = user32Wrapper;
                display = user32Wrapper;

                audio = new NAudioProvider(DefaultLogger);

                Autostart = new WinAutostartService();
                UserInterface = new ConsoleUI();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ydotoolWrapper = new YdotoolProvider(DefaultLogger);
                var dummyWrapper = new DummyProvider(DefaultLogger);

                keyboard = ydotoolWrapper;
                mouse = ydotoolWrapper;

                display = dummyWrapper;
                audio = dummyWrapper;

                Autostart = new DummyAutostartService();
                UserInterface = new ConsoleUI();
            }
            else
            {
                throw new Exception("OS not supported");
            }

            var listener = new GenericListener(DefaultLogger);

            var controllers = new BaseController[]
            {
                new AudioController(audio, DefaultLogger),
                new DisplayController(display, DefaultLogger),
                new KeyboardController(keyboard, DefaultLogger),
                new MouseController(mouse, DefaultLogger)
            };

            var endPoint = new ApiEndpointV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);

            Server = new SimpleServer(listener, staticMiddleware);
            Config = new LocalFileConfigProvider(DefaultLogger);
        }
    }
}