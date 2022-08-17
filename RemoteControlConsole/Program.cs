using System.Runtime.InteropServices;
using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Web.Controllers;
using RemoteControlApp.Web.Middleware;
using Shared.Interfaces.Control;

var consoleLogger = new ConsoleLogger();

IKeyboardControl keyboardControl;
IMouseControl mouseControl;
IDisplayControl displayControl;
IAudioControl audioControl;

if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    var wrapper = new User32Wrapper(consoleLogger);

    keyboardControl = wrapper;
    mouseControl = wrapper;
    displayControl = wrapper;

    audioControl = new NAudioWrapper(consoleLogger);
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    var wrapper = new YdotoolWrapper(consoleLogger);

    keyboardControl = wrapper;
    mouseControl = wrapper;

    var dummy = new DummyWrapper(consoleLogger);
    displayControl = dummy;
    audioControl = dummy;
}

else
{
    Console.WriteLine("OS not supported");
    return;
}

var uiListener = new GenericListener(consoleLogger);
var apiListener = new GenericListener(consoleLogger);

var controllers = new BaseController[]
{
    new AudioController(audioControl, consoleLogger),
    new DisplayController(displayControl, consoleLogger),
    new KeyboardController(keyboardControl, consoleLogger),
    new MouseController(mouseControl, consoleLogger)
};

var uiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new FileMiddleware()).GetFirst();
var apiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new ApiMiddlewareV1(controllers)).GetFirst();

var app = new RemoteControl(uiListener, apiListener, uiMiddlewareChain, apiMiddlewareChain);
var config = new LocalFileConfigService(consoleLogger);

app.Start(config.GetConfig().UriConfig.Uri);

Console.ReadLine();
