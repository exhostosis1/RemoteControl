using MainApp.ControlProviders;
using MainApp.Servers;
using MainApp.Servers.ApiControllers;
using MainApp.Servers.Listeners;
using MainApp.Servers.Middleware;
using Microsoft.Extensions.Logging;

namespace MainApp;

public sealed class ServerFactory
{
    private readonly IMiddleware[] _webMiddlewareChain;
    private readonly IMiddleware[] _botMiddlewareChain;
    private readonly ILoggerProvider _loggerProvider;

    public ServerFactory(ILoggerProvider loggingProvider)
    {
        _loggerProvider = loggingProvider;

        var inputProvider = new User32Wrapper();
        var audioProvider = new NAudioWrapper();

        var audioController = new AudioController(audioProvider, loggingProvider.CreateLogger(nameof(AudioController)));
        var mouseController = new MouseController(inputProvider, loggingProvider.CreateLogger(nameof(MouseController)));
        var keyboardController = new KeyboardController(inputProvider, loggingProvider.CreateLogger(nameof(KeyboardController)));
        var displayController = new DisplayController(inputProvider, loggingProvider.CreateLogger(nameof(DisplayController)));

        var staticMiddleware = new StaticFilesMiddleware(loggingProvider.CreateLogger(nameof(StaticFilesMiddleware)));

        var apiMiddleware =
            new ApiV1Middleware([audioController, mouseController, keyboardController, displayController],
                loggingProvider.CreateLogger(nameof(ApiV1Middleware)));

        _webMiddlewareChain = [apiMiddleware, staticMiddleware];
        _botMiddlewareChain = [apiMiddleware];
    }

    public Server GetServer(ServerConfig config)
    {
        return new Server(config,
            config.Type == ServerType.Web
                ? new SimpleHttpListener(_loggerProvider.CreateLogger(nameof(SimpleHttpListener)))
                : new TelegramListener(_loggerProvider.CreateLogger(nameof(TelegramListener))),
            config.Type == ServerType.Web ? _webMiddlewareChain : _botMiddlewareChain,
            _loggerProvider.CreateLogger(nameof(Server)));
    }
}