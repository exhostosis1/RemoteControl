using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Web.Controllers;
using RemoteControlApp.Web.Middleware;

var consoleLogger = new ConsoleLogger();

var dummy = new DummyWrapper(consoleLogger);

var uiListener = new GenericListener(consoleLogger);
var apiListener = new GenericListener(consoleLogger);

var controllers = new BaseController[]
{
    new AudioController(dummy, consoleLogger),
    new DisplayController(dummy, consoleLogger),
    new KeyboardController(dummy, consoleLogger),
    new MouseController(dummy, consoleLogger)
};

var uiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new FileMiddleware()).GetFirst();
var apiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new ApiMiddlewareV1(controllers)).GetFirst();

var app = new RemoteControl(uiListener, apiListener, uiMiddlewareChain, apiMiddlewareChain);
var config = new LocalFileConfigService(consoleLogger);

app.Start(config.GetConfig().UriConfig.Uri);

while(true) Console.ReadLine();
