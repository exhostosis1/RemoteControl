using Autostart;
using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Web.Controllers;
using RemoteControlApp.Web.Middleware;
using Shared.Enums;

var consoleLogger = new ConsoleLogger(LoggingLevel.Info);

var audio = new NAudioWrapper();
var input = new User32Wrapper();

var uiListener = new GenericListener(consoleLogger);
var apiListener = new GenericListener(consoleLogger);

var controllers = new BaseController[]
{
    new AudioController(audio, consoleLogger),
    new KeyboardController(input, consoleLogger),
    new MouseController(input, consoleLogger),
    new DisplayController(input, consoleLogger)
};

var uiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new FileMiddleware()).GetFirst();
var apiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new ApiMiddlewareV1(controllers)).GetFirst();

var app = new RemoteControl(uiListener, apiListener, uiMiddlewareChain, apiMiddlewareChain);
var config = new LocalFileConfigService(consoleLogger);
var autostart = new WinAutostartService();

app.Start(config.GetConfig().UriConfig.Uri);

while(true) Console.ReadLine();
