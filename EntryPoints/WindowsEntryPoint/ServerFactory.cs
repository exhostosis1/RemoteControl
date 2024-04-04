using ApiControllers;
using ControlProviders;
using ControlProviders.Wrappers;
using Listeners;
using Microsoft.Extensions.Logging;
using Servers;
using Servers.Middleware;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.Listener;
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
    private readonly ILoggerProvider _loggerProvider;

    public ServerFactory(ILoggerProvider loggingProvider)
    {
        _loggerProvider = loggingProvider;

        var inputProvider = new User32Wrapper();
        var audioProvider = new NAudioWrapper();

        var generalInput = new InputProvider(inputProvider, inputProvider, inputProvider, audioProvider);

        var audioController = new AudioController(generalInput, loggingProvider.CreateLogger(nameof(AudioController)));
        var mouseController = new MouseController(generalInput, loggingProvider.CreateLogger(nameof(MouseController)));
        var keyboardController = new KeyboardController(generalInput, loggingProvider.CreateLogger(nameof(KeyboardController)));
        var displayController = new DisplayController(generalInput, loggingProvider.CreateLogger(nameof(DisplayController)));

        var staticMiddleware = new StaticFilesMiddleware(loggingProvider.CreateLogger(nameof(StaticFilesMiddleware)));

        var apiMiddleware =
            new ApiV1Middleware([audioController, mouseController, keyboardController, displayController],
                loggingProvider.CreateLogger(nameof(ApiV1Middleware)));

        _webMiddlewareChain = new WebMiddlewareChain([apiMiddleware, staticMiddleware]);
        _webListener = new SimpleHttpListener(new HttpListenerWrapper(loggingProvider.CreateLogger(nameof(HttpListenerWrapper))),
            loggingProvider.CreateLogger(nameof(SimpleHttpListener)));

        _botListener = new TelegramListener(new TelegramBotApiProvider(new HttpClientWrapper()),
            loggingProvider.CreateLogger(nameof(TelegramListener)));
        _botMiddlewareChain = new BotMiddlewareChain([new CommandsExecutor(generalInput, loggingProvider.CreateLogger(nameof(CommandsExecutor)))]);
    }

    public SimpleServer GetServer()
    {
        return new SimpleServer(_webListener, _webMiddlewareChain, _loggerProvider.CreateLogger(nameof(SimpleServer)));
    }

    public SimpleServer GetServer(WebConfig config, int id = 0)
    {
        var server = GetServer();
        server.CurrentConfig = config;
        server.Id = id;

        return server;
    }

    public BotServer GetBot()
    {
        return new BotServer(_botListener, _botMiddlewareChain, _loggerProvider.CreateLogger(nameof(BotServer)));
    }
    
    public BotServer GetBot(BotConfig config, int id)
    {
        var bot = GetBot();
        bot.CurrentConfig = config;
        bot.Id = id;

        return bot;
    }
}