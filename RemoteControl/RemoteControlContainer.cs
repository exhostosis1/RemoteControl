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
using Shared.Logging.Interfaces;

namespace RemoteControl
{
    public class RemoteControlContainer : IContainer
    {
        public IServer Server { get; }
        public IConfigProvider Config { get; }
        public IAutostartService Autostart { get; }
        public ILogger DefaultLogger { get; }

        public RemoteControlContainer()
        {
            var fileLogger = new FileLogger("error.log");

            DefaultLogger = fileLogger;

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

                Autostart = new WinAutostartService();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ydotoolWrapper = new YdotoolProvider(fileLogger);
                var dummyWrapper = new DummyProvider(fileLogger);

                keyboard = ydotoolWrapper;
                mouse = ydotoolWrapper;

                display = dummyWrapper;
                audio = dummyWrapper;

                Autostart = new DummyAutostartService();
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

            Server = new SimpleServer(listener, staticMiddleware);
            Config = new LocalFileConfigProvider(fileLogger);
        }
    }
}