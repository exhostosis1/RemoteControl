using ApiControllers;
using Bots;
using Listeners;
using Servers.Endpoints;
using Shared;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;
using Shared.Wrappers;

namespace RemoteControlMain;

internal class Container : IContainer
{
    private readonly IPlatformDependantContainer _innerContainer;

    public IConfigProvider ConfigProvider => _innerContainer.ConfigProvider;
    public IAutostartService AutostartService => _innerContainer.AutostartService;
    public IUserInterface UserInterface => _innerContainer.UserInterface;
    public IControlProvider ControlProvider => _innerContainer.ControlProvider;
    public ILogger Logger => _innerContainer.Logger;
    public IListener<HttpContext> WebListener { get; }
    public IListener<BotContext> BotListener { get; }
    public IHttpClient HttpClient { get; }
    public IHttpListener HttpListener { get; }
    public TelegramBotApiProvider TelegramBotApiProvider { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public AbstractMiddleware<HttpContext> ApiMiddleware { get; }
    public AbstractMiddleware<HttpContext> StaticMiddleware { get; }
    public AbstractMiddleware<BotContext> CommandExecutor { get; }

    public ILogger NewLogger() => _innerContainer.NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger) => _innerContainer.NewConfigProvider(logger);
    public IAutostartService NewAutostartService(ILogger logger) => _innerContainer.NewAutostartService(logger);
    public IUserInterface NewUserInterface() => _innerContainer.NewUserInterface();
    public IControlProvider NewControlProvider(ILogger logger) => _innerContainer.NewControlProvider(logger);
    public IListener<HttpContext> NewWebListener(IHttpListener listener) => new SimpleHttpListener(listener);
    public IListener<BotContext> NewBotListener(TelegramBotApiProvider provider, ILogger logger) =>
        new TelegramListener(provider, new LogWrapper<TelegramListener>(logger));
    public IHttpClient NewHttpClient() => new HttpClientWrapper();
    public IHttpListener NewHttpListener() => new HttpListenerWrapper();
    public TelegramBotApiProvider NewTelegramBotApiProvider(IHttpClient client, ILogger logger) =>
        new(client, new LogWrapper<TelegramBotApiProvider>(logger));
    public IApiController NewAudioController(IControlProvider provider, ILogger logger) =>
        new AudioController(provider, new LogWrapper<AudioController>(logger));
    public IApiController NewKeyboardController(IControlProvider provider, ILogger logger) =>
        new KeyboardController(provider, new LogWrapper<KeyboardController>(logger));
    public IApiController NewMouseController(IControlProvider provider, ILogger logger) =>
        new MouseController(provider, new LogWrapper<MouseController>(logger));
    public IApiController NewDisplayController(IControlProvider provider, ILogger logger) =>
        new DisplayController(provider, new LogWrapper<DisplayController>(logger));
    public AbstractMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger,
        AbstractMiddleware<HttpContext>? next = null) =>
        new ApiV1Middleware(controllers, new LogWrapper<ApiV1Middleware>(logger), next);
    public AbstractMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www") =>
        new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(logger), directory);
    public AbstractMiddleware<BotContext> NewCommmandExecutor(IControlProvider provider, ILogger logger) =>
        new CommandsExecutor(provider, new LogWrapper<CommandsExecutor>(logger));

    public Container(IPlatformDependantContainer inner)
    {
        _innerContainer = inner;

        HttpListener = NewHttpListener();
        WebListener = NewWebListener(HttpListener);
        
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