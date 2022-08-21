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

namespace RemoteControl
{
    public static class RemoteControlMain
    {
        public static IServer Server { get; private set; }
        public static IConfigProvider Config { get; private set; }
        public static IAutostartService AutostartService { get; private set; }

        static RemoteControlMain()
        {
            var fileLogger = new FileLogger("error.log");
            var consoleLogger = new ConsoleLogger();

            IKeyboardControlProvider keyboard;
            IMouseControlProvider mouse;
            IDisplayControlProvider display;
            IAudioControlProvider audio;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var user32Wrapper = new User32Provider(fileLogger);

                keyboard = user32Wrapper;
                mouse = user32Wrapper;
                display = user32Wrapper;

                audio = new NAudioProvider(fileLogger);

                AutostartService = new WinAutostartService();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ydotoolWrapper = new YdotoolProvider(fileLogger);
                var dummyWrapper = new DummyProvider(fileLogger);

                keyboard = ydotoolWrapper;
                mouse = ydotoolWrapper;

                display = dummyWrapper;
                audio = dummyWrapper;

                AutostartService = new DummyAutostartService();
            }
            else
            {
                throw new Exception("OS not supported");
            }

            var listener = new GenericListener(fileLogger);

            var controllers = new BaseController[]
            {
                new AudioController(audio, fileLogger),
                new DisplayController(display, fileLogger),
                new KeyboardController(keyboard, fileLogger),
                new MouseController(mouse, fileLogger)
            };

            var endPoint = new ApiEndpointV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);
            var loggingMiddleware = new LoggingMiddleware(staticMiddleware.ProcessRequest, consoleLogger);

            Server = new SimpleServer(listener, loggingMiddleware);
            Config = new LocalFileConfigProvider(fileLogger);
        }
    }
}
