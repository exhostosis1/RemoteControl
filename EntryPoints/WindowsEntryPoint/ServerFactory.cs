using ApiControllers;
using ControlProviders;
using ControlProviders.Wrappers;
using Listeners;
using Microsoft.Extensions.Logging;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.Wrappers.HttpListener;

namespace MainApp;

public class ServerFactory
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

        var generalInput = new InputProvider(inputProvider, inputProvider, inputProvider, audioProvider);

        var audioController = new AudioController(generalInput, loggingProvider.CreateLogger(nameof(AudioController)));
        var mouseController = new MouseController(generalInput, loggingProvider.CreateLogger(nameof(MouseController)));
        var keyboardController = new KeyboardController(generalInput, loggingProvider.CreateLogger(nameof(KeyboardController)));
        var displayController = new DisplayController(generalInput, loggingProvider.CreateLogger(nameof(DisplayController)));

        var staticMiddleware = new StaticFilesMiddleware(loggingProvider.CreateLogger(nameof(StaticFilesMiddleware)));

        var apiMiddleware =
            new ApiV1Middleware([audioController, mouseController, keyboardController, displayController],
                loggingProvider.CreateLogger(nameof(ApiV1Middleware)));

        _webMiddlewareChain = [apiMiddleware, staticMiddleware];
        _webListener = new SimpleHttpListener(new HttpListenerWrapper(loggingProvider.CreateLogger(nameof(HttpListenerWrapper))), loggingProvider.CreateLogger(nameof(SimpleHttpListener)));

        _botListener = new TelegramListener(new TelegramBotApiProvider(), loggingProvider.CreateLogger(nameof(TelegramListener)));
        _botMiddlewareChain = [new CommandsExecutor(generalInput, loggingProvider.CreateLogger(nameof(CommandsExecutor)))];
    }

    public Server GetServer()
    {
        return new Server(ServerType.Web, _webListener, _webMiddlewareChain, _loggerProvider.CreateLogger(nameof(Server)));
    }

    public Server GetServer(ServerConfig config, int id = 0)
    {
        var server = GetServer();
        server.Config = config;
        server.Id = id;

        return server;
    }

    public Server GetBot()
    {
        return new Server(ServerType.Bot, _botListener, _botMiddlewareChain, _loggerProvider.CreateLogger(nameof(Server)));
    }
    
    public Server GetBot(ServerConfig config, int id)
    {
        var bot = GetBot();
        bot.Config = config;
        bot.Id = id;

        return bot;
    }
}