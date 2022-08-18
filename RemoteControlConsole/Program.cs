using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using Shared.Interfaces.Control;
using Shared.Interfaces.Web;
using System.Runtime.InteropServices;
using RemoteControlApp.Middleware;
using WebApiProvider.Controllers;

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

var controllers = new IController[]
{
    new AudioController(audio, consoleLogger),
    new DisplayController(display, consoleLogger),
    new KeyboardController(keyboard, consoleLogger),
    new MouseController(mouse, consoleLogger)
};

var endPoint = new ApiEndpointV1(controllers);
var staticMiddleware = new StaticFilesMiddleware(endPoint.ProcessRequest);
var loggingMiddleware = new LoggingMiddleware(staticMiddleware.ProcessRequest, consoleLogger);

var app = new RemoteControl(listener, loggingMiddleware.ProcessRequest);

var config = new LocalFileConfigService(consoleLogger);

app.Start(config.GetConfig().UriConfig.Uri);

Console.ReadLine();