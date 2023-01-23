using ApiControllers;
using Listeners;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.ControlProviders.Provider;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;

namespace RemoteControlMain;

internal class Container : IContainer
{
    private readonly IPlatformDependantContainer _innerContainer;

    public IConfigProvider ConfigProvider => _innerContainer.ConfigProvider;
    public IAutostartService AutostartService => _innerContainer.AutostartService;
    public IUserInterface UserInterface => _innerContainer.UserInterface;
    public IGeneralControlProvider ControlProvider => _innerContainer.ControlProvider;
    public ILogger Logger => _innerContainer.Logger;
    public IListener<HttpContext> WebListener { get; }
    public IListener<BotContext> BotListener { get; }
    public IHttpClient HttpClient { get; }
    public IHttpListener HttpListener { get; }
    public IBotApiProvider TelegramBotApiProvider { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public IMiddleware<HttpContext> ApiMiddleware { get; }
    public IMiddleware<HttpContext> StaticMiddleware { get; }
    public IMiddleware<BotContext> CommandExecutor { get; }

    public ILogger NewLogger() => _innerContainer.NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger) => _innerContainer.NewConfigProvider(logger);
    public IAutostartService NewAutostartService(ILogger logger) => _innerContainer.NewAutostartService(logger);
    public IUserInterface NewUserInterface() => _innerContainer.NewUserInterface();
    public IGeneralControlProvider NewControlProvider(ILogger logger) => _innerContainer.NewControlProvider(logger);
    public IListener<HttpContext> NewWebListener(IHttpListener listener, ILogger logger) =>
        new SimpleHttpListener(listener, new LogWrapper<SimpleHttpListener>(logger));
    public IListener<BotContext> NewBotListener(IBotApiProvider provider, ILogger logger) =>
        new TelegramListener(provider, new LogWrapper<TelegramListener>(logger));
    public IHttpClient NewHttpClient() => new HttpClientWrapper();
    public IHttpListener NewHttpListener(ILogger logger) => new HttpListenerWrapper(new LogWrapper<HttpListenerWrapper>(logger));
    public IBotApiProvider NewTelegramBotApiProvider(IHttpClient client, ILogger logger) =>
        new TelegramBotApiProvider(client, new LogWrapper<TelegramBotApiProvider>(logger));
    public IApiController NewAudioController(IAudioControlProvider provider, ILogger logger) =>
        new AudioController(provider, new LogWrapper<AudioController>(logger));
    public IApiController NewKeyboardController(IKeyboardControlProvider provider, ILogger logger) =>
        new KeyboardController(provider, new LogWrapper<KeyboardController>(logger));
    public IApiController NewMouseController(IMouseControlProvider provider, ILogger logger) =>
        new MouseController(provider, new LogWrapper<MouseController>(logger));
    public IApiController NewDisplayController(IDisplayControlProvider provider, ILogger logger) =>
        new DisplayController(provider, new LogWrapper<DisplayController>(logger));
    public IMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger,
        IMiddleware<HttpContext>? next = null) =>
        new ApiV1Middleware(controllers, new LogWrapper<ApiV1Middleware>(logger), next);
    public IMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www") =>
        new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(logger), directory);
    public IMiddleware<BotContext> NewCommmandExecutor(IGeneralControlProvider provider, ILogger logger) =>
        new CommandsExecutor(provider, new LogWrapper<CommandsExecutor>(logger));

    public Container(IPlatformDependantContainer inner)
    {
        _innerContainer = inner;

        HttpListener = NewHttpListener(Logger);
        WebListener = NewWebListener(HttpListener, Logger);

        HttpClient = NewHttpClient();
        TelegramBotApiProvider = NewTelegramBotApiProvider(HttpClient, Logger);
        BotListener = NewBotListener(TelegramBotApiProvider, Logger);

        AudioController = NewAudioController(ControlProvider, Logger);
        DisplayController = NewDisplayController(ControlProvider, Logger);
        KeyboardController = NewKeyboardController(ControlProvider, Logger);
        MouseController = NewMouseController(ControlProvider, Logger);

        StaticMiddleware = NewStaticMiddleware(Logger);
        ApiMiddleware =
            NewApiMiddleware(new[] { AudioController, DisplayController, MouseController, KeyboardController }, Logger,
                StaticMiddleware);
        CommandExecutor = NewCommmandExecutor(ControlProvider, Logger);
    }
}