using ApiControllers;
using Listeners;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.ControlProviders.Input;
using Shared.ControlProviders.Provider;
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
    public IKeyboardInput KeyboardInput => _innerContainer.KeyboardInput;
    public IMouseInput MouseInput => _innerContainer.MouseInput;
    public IDisplayInput DisplayInput => _innerContainer.DisplayInput;
    public IAudioInput AudioInput => _innerContainer.AudioInput;
    public ILogger Logger => _innerContainer.Logger;
    public IWebListener WebListener { get; }
    public IBotListener BotListener { get; }
    public IHttpClient HttpClient { get; }
    public IHttpListener HttpListener { get; }
    public IBotApiProvider TelegramBotApiProvider { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public IWebMiddleware ApiMiddleware { get; }
    public IWebMiddleware StaticMiddleware { get; }
    public IBotMiddleware CommandExecutor { get; }

    public ILogger NewLogger() => _innerContainer.NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger) => _innerContainer.NewConfigProvider(logger);
    public IAutostartService NewAutostartService(ILogger logger) => _innerContainer.NewAutostartService(logger);
    public IUserInterface NewUserInterface() => _innerContainer.NewUserInterface();
    public IGeneralControlProvider NewControlProvider(ILogger logger) => _innerContainer.NewControlProvider(logger);
    public IKeyboardInput NewKeyboardInput() => _innerContainer.NewKeyboardInput();
    public IMouseInput NewMouseInput() => _innerContainer.NewMouseInput();
    public IDisplayInput NewDisplayInput() => _innerContainer.NewDisplayInput();
    public IAudioInput NewAudioInput() => _innerContainer.NewAudioInput();

    public IWebListener NewWebListener(IHttpListener listener, ILogger logger) =>
        new SimpleHttpListener(listener, new LogWrapper<SimpleHttpListener>(logger));
    public IBotListener NewBotListener(IBotApiProvider provider, ILogger logger) =>
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
    public IWebMiddleware NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger,
        IWebMiddleware? next = null) =>
        new ApiV1Middleware(controllers, new LogWrapper<ApiV1Middleware>(logger), next);
    public IWebMiddleware NewStaticMiddleware(ILogger logger, string directory = "www") =>
        new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(logger), directory);
    public IBotMiddleware NewCommmandExecutor(IGeneralControlProvider provider, ILogger logger) =>
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