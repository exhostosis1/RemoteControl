using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using System.Runtime.InteropServices;
using RemoteControlApp.Controllers;
using Server;
using Server.Middleware;
using Shared.Control;
using Shared.Controllers;

namespace RemoteControlApp
{
    internal static class RemoteControlApp
    {
        public static void Main()
        {
            var consoleLogger = new ConsoleLogger();
            IKeyboardControl keyboard;
            IMouseControl mouse;
            IDisplayControl display;
            IAudioControl audio;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var user32Wrapper = new User32Wrapper(consoleLogger);

                keyboard = user32Wrapper;
                mouse = user32Wrapper;
                display = user32Wrapper;

                audio = new NAudioWrapper(consoleLogger);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ydotoolWrapper = new YdotoolWrapper(consoleLogger);
                var dummyWrapper = new DummyWrapper(consoleLogger);

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

            var config = new LocalFileConfigService(consoleLogger);

            server.Start(config.GetConfig().UriConfig.Uri);

            Console.ReadLine();
        }
    }
}
