using ApiControllers;
using Bots;
using Listeners;
using Listeners.Wrappers;
using Listeners.Wrappers.Telegram;
using Servers.Endpoints;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;

namespace RemoteControlMain;

internal class Container : IContainer
{
    private readonly IPlatformDependantContainer _container;

    public IConfigProvider ConfigProvider => _container.ConfigProvider;
    public IAutostartService AutostartService => _container.AutostartService;
    public IUserInterface UserInterface => _container.UserInterface;
    public IControlProvider ControlProvider => _container.ControlProvider;
    public IListener<HttpContext> HttpListener { get; }
    public IListener<BotContext> BotListener { get; }
    public IHttpListenerWrapper HttpWrapper { get; }
    public IActiveApiWrapper ActiveBotWrapper { get; }
    public ICommandExecutor Executor { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public AbstractMiddleware<HttpContext> ApiMiddleware { get; }
    public AbstractMiddleware<HttpContext> StaticMiddleware { get; }
    public ILogger Logger => _container.Logger;
    public ILogger NewLogger() => _container.NewLogger();

    public IListener<HttpContext> NewHttpListener(IHttpListenerWrapper wrapper, ILogger logger) =>
        new SimpleHttpListener(wrapper, new LogWrapper<SimpleHttpListener>(logger));

    public IListener<BotContext> NewBotListener(IActiveApiWrapper wrapper, ILogger logger) =>
        new ActiveBotListener(wrapper, new LogWrapper<ActiveBotListener>(logger));

    public IActiveApiWrapper NewBotWrapper(ILogger logger) =>
        new TelegramBotApiWrapper(new LogWrapper<TelegramBotApiWrapper>(logger));

    public IHttpListenerWrapper NewHttpWrapper() =>
        new HttpListenerWrapper();

    public ICommandExecutor NewExecutor(IControlProvider facade, ILogger logger) =>
        new CommandsExecutor(facade, new LogWrapper<CommandsExecutor>(logger));

    public IApiController NewAudioController(IControlProvider provider, ILogger logger) =>
        new AudioController(provider, new LogWrapper<AudioController>(logger));
    public IApiController NewKeyboardController(IControlProvider provider, ILogger logger) =>
        new KeyboardController(provider, new LogWrapper<KeyboardController>(logger));
    public IApiController NewMouseController(IControlProvider provider, ILogger logger) =>
        new MouseController(provider, new LogWrapper<MouseController>(logger));
    public IApiController NewDisplayController(IControlProvider provider, ILogger logger) =>
        new DisplayController(provider, new LogWrapper<DisplayController>(logger));

    public AbstractMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger, AbstractMiddleware<HttpContext>? next = null) =>
        new ApiV1Middleware(controllers, new LogWrapper<ApiV1Middleware>(logger), next);

    public AbstractMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www") =>
        new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(logger), directory);

    public IConfigProvider NewConfigProvider(ILogger logger) => _container.NewConfigProvider(logger);

    public IAutostartService NewAutostartService(ILogger logger) => _container.NewAutostartService(logger);

    public IUserInterface NewUserInterface() => _container.NewUserInterface();

    public IControlProvider NewControlProvider(ILogger logger) => _container.NewControlProvider(logger);

    public Container(IPlatformDependantContainer input)
    {
        _container = input;

        AudioController = NewAudioController(ControlProvider, Logger);
        DisplayController = NewDisplayController(ControlProvider, Logger);
        MouseController = NewMouseController(ControlProvider, Logger);
        KeyboardController = NewKeyboardController(ControlProvider, Logger);

        StaticMiddleware = NewStaticMiddleware(Logger);
        ApiMiddleware = NewApiMiddleware(new[] { AudioController, DisplayController, MouseController, KeyboardController }, Logger, StaticMiddleware);

        Executor = NewExecutor(ControlProvider, Logger);

        HttpWrapper = NewHttpWrapper();
        ActiveBotWrapper = NewBotWrapper(Logger);

        HttpListener = NewHttpListener(HttpWrapper, Logger);
        BotListener = NewBotListener(ActiveBotWrapper, Logger);
    }
}