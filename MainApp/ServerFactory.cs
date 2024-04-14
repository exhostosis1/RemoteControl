using ControlProviders;
using Microsoft.Extensions.Logging;
using Servers;
using Servers.ApiControllers;
using Servers.Listeners;
using Servers.Middleware;
using User32Wrapper = ControlProviders.User32Wrapper;

namespace MainApp;

public sealed class ServerFactory
{
    private readonly SimpleHttpListener _webListener;
    private readonly IMiddleware[] _webMiddlewareChain;
    private readonly TelegramListener _botListener;
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
        _webListener = new SimpleHttpListener(loggingProvider.CreateLogger(nameof(SimpleHttpListener)));

        _botListener = new TelegramListener(loggingProvider.CreateLogger(nameof(TelegramListener)));
        _botMiddlewareChain = [apiMiddleware];
    }

    public Server GetServer(ServerConfig? config = null)
    {
        return new Server(ServerType.Web, _webListener, _webMiddlewareChain, _loggerProvider.CreateLogger(nameof(Server)), config);
    }

    public Server GetBot(ServerConfig? config = null)
    {
        return new Server(ServerType.Bot, _botListener, _botMiddlewareChain, _loggerProvider.CreateLogger(nameof(Server)), config);
    }
}