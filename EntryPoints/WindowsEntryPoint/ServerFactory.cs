using ApiControllers;
using ControlProviders;
using ControlProviders.Wrappers;
using Listeners;
using Servers;
using Servers.Middleware;
using Shared.Bots.Telegram;
using Shared.Listener;
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

        var audioController = new AudioController(generalInput, _logger.WrapLogger<AudioController>());
        var mouseController = new MouseController(generalInput, _logger.WrapLogger<MouseController>());
        var keyboardController = new KeyboardController(generalInput, _logger.WrapLogger<KeyboardController>());
        var displayController = new DisplayController(generalInput, _logger.WrapLogger<DisplayController>());

        var staticMiddleware = new StaticFilesMiddleware(_logger.WrapLogger<StaticFilesMiddleware>());
        var loggingMiddleware = new LoggingMiddleware(_logger.WrapLogger<LoggingMiddleware>());
        var apiMiddleware =
            new ApiV1Middleware([audioController, mouseController, keyboardController, displayController],
                _logger.WrapLogger<ApiV1Middleware>());

        _webMiddlewareChain = new WebMiddlewareChain([loggingMiddleware, apiMiddleware, staticMiddleware]);
        _webListener = new SimpleHttpListener(new HttpListenerWrapper(_logger.WrapLogger<HttpListenerWrapper>()),
            _logger.WrapLogger<SimpleHttpListener>());

        _botListener = new TelegramListener(new TelegramBotApiProvider(new HttpClientWrapper()),
            _logger.WrapLogger<TelegramListener>());
        _botMiddlewareChain = new BotMiddlewareChain([new CommandsExecutor(generalInput, _logger.WrapLogger<CommandsExecutor>())]);
    }

    public SimpleServer GetServer()
    {
        return new SimpleServer(_webListener, _webMiddlewareChain, _logger.WrapLogger<SimpleServer>());
    }
    
    public BotServer GetBot()
    {
        return new BotServer(_botListener, _botMiddlewareChain, _logger.WrapLogger<BotServer>());
    }
}