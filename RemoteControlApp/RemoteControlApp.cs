using ConfigProviders;
using ControlProviders;
using Listeners;
using Logging;
using RemoteControlApp.Controllers;
using Servers;
using Servers.Middleware;
using Shared.Controllers;
using Shared.ControlProviders;
using System.Runtime.InteropServices;

namespace RemoteControlApp
{
    internal static class RemoteControlApp
    {
        public static void Main()
        {
            var consoleLogger = new ConsoleLogger();
            IKeyboardControlProvider keyboard;
            IMouseControlProvider mouse;
            IDisplayControlProvider display;
            IAudioControlProvider audio;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var user32Wrapper = new User32Provider(consoleLogger);

                keyboard = user32Wrapper;
                mouse = user32Wrapper;
                display = user32Wrapper;

                audio = new NAudioProvider(consoleLogger);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ydotoolWrapper = new YdotoolProvider(consoleLogger);
                var dummyWrapper = new DummyProvider(consoleLogger);

                keyboard = ydotoolWrapper;
                mouse = ydotoolWrapper;

                display = dummyWrapper;
                audio = dummyWrapper;
            }
            else
            {
                Console.WriteLine("OS not supported");
                return;
            }

            var listener = new GenericListener(consoleLogger);

            var controllers = new BaseController[]
            {
                new AudioController(audio, consoleLogger),
                new DisplayController(display, consoleLogger),
                new KeyboardController(keyboard, consoleLogger),
                new MouseController(mouse, consoleLogger)
            };

            var endPoint = new ApiEndpointV1(controllers);
            var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);
            var loggingMiddleware = new LoggingMiddleware(staticMiddleware.ProcessRequest, consoleLogger);

            var server = new SimpleServer(listener, loggingMiddleware);

            var config = new LocalFileConfigProvider(consoleLogger);

            server.Start(config.GetConfig().UriConfig.Uri);

            Console.ReadLine();
        }
    }
}
