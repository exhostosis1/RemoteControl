using ApiControllers;
using ControlProviders;
using ControlProviders.Wrappers;
using Listeners;
using Servers;
using Servers.Middleware;
using Shared.Bots.Telegram;
using Shared.Listener;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;

namespace WindowsEntryPoint;

public class ServerFactory
{
    private readonly IWebListener _webListener;
    private readonly IWebMiddlewareChain _webMiddlewareChain;
    private readonly IBotListener _botListener;
    private readonly IBotMiddlewareChain _botMiddlewareChain;
    private readonly ILogger _logger;

    public ServerFactory(ILogger logger)
    {
        _logger = logger;

        var inputProvider = new User32Wrapper();
        var audioProvider = new NAudioWrapper();

        var generalInput = new InputProvider(inputProvider, inputProvider, inputProvider, audioProvider);

        var audioController = new AudioController(generalInput, new LogWrapper<AudioController>(_logger));
        var mouseController = new MouseController(generalInput, new LogWrapper<MouseController>(_logger));
        var keyboardController = new KeyboardController(generalInput, new LogWrapper<KeyboardController>(_logger));
        var displayController = new DisplayController(generalInput, new LogWrapper<DisplayController>(_logger));

        var staticMiddleware = new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(_logger));
        var loggingMiddleware = new LoggingMiddleware(new LogWrapper<LoggingMiddleware>(_logger));
        var apiMiddleware =
            new ApiV1Middleware([audioController, mouseController, keyboardController, displayController],
                new LogWrapper<ApiV1Middleware>(_logger));

        _webMiddlewareChain = new WebMiddlewareChain([loggingMiddleware, apiMiddleware, staticMiddleware]);
        _webListener = new SimpleHttpListener(new HttpListenerWrapper(new LogWrapper<HttpListenerWrapper>(_logger)),
            new LogWrapper<SimpleHttpListener>(_logger));

        _botListener = new TelegramListener(new TelegramBotApiProvider(new HttpClientWrapper()),
            new LogWrapper<TelegramListener>(_logger));
        _botMiddlewareChain = new BotMiddlewareChain([new CommandsExecutor(generalInput, new LogWrapper<CommandsExecutor>(_logger))]);
    }

    public SimpleServer GetServer()
    {
        return new SimpleServer(_webListener, _webMiddlewareChain, new LogWrapper<SimpleServer>(_logger));
    }
    
    public BotServer GetBot()
    {
        return new BotServer(_botListener, _botMiddlewareChain, new LogWrapper<BotServer>(_logger));
    }
}