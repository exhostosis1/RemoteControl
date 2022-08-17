using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using Shared.Interfaces.Control;
using System.Runtime.InteropServices;
using WebApiProvider;
using WebApiProvider.Controllers;
using WebUiProvider;

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

var listener = new GenericListener(consoleLogger);

var controllers = new BaseController[]
{
    new AudioController(audioControl, consoleLogger),
    new DisplayController(displayControl, consoleLogger),
    new KeyboardController(keyboardControl, consoleLogger),
    new MouseController(mouseControl, consoleLogger)
};

var app = new RemoteControl(listener)
    .Use((context, next) =>
    {
        Console.WriteLine("placeholder before");
        next(context);
        Console.WriteLine("placeholder after");
    })
    .UseMiddleware<LoggingMiddleware>(consoleLogger)
    .UseMiddleware<ApiMiddlewareV1>((object)controllers)
    .UseMiddleware<FileMiddleware>()
    .Build();

var config = new LocalFileConfigService(consoleLogger);

app.Start(config.GetConfig().UriConfig.Uri);

Console.ReadLine();
