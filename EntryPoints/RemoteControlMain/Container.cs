using ApiControllers;
using Bots;
using Bots.Telegram;
using Listeners;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;

namespace RemoteControlMain;

internal class Container : IContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public ControlFacade ControlProviders { get; }
    public IHttpListener HttpListener { get; }
    public IBotListener BotListener { get; }
    public AbstractMiddleware Middleware { get; }

    private readonly IPlatformDependantContainer _innerContainer;

    public ILogger GetLogger(Type type) => _innerContainer.GetLogger(type);

    public Container(IPlatformDependantContainer input)
    {
        _innerContainer = input;

        ConfigProvider = input.ConfigProvider;
        AutostartService = input.AutostartService;
        UserInterface = input.UserInterface;
        ControlProviders = input.ControlProviders;
        HttpListener = new SimpleHttpListener(input.GetLogger(typeof(SimpleHttpListener)));

        var controllers = new BaseApiController[]
        {
            new AudioController(ControlProviders.Audio, GetLogger(typeof(AudioController))),
            new DisplayController(ControlProviders.Display, GetLogger(typeof(DisplayController))),
            new KeyboardController(ControlProviders.Keyboard, GetLogger(typeof(KeyboardController))),
            new MouseController(ControlProviders.Mouse, GetLogger(typeof(MouseController)))
        };
        var apiEndpoint = new ApiV1Endpoint(controllers, GetLogger(typeof(ApiV1Endpoint)));
        var staticEndpoint = new StaticFilesEndpoint(GetLogger(typeof(StaticFilesEndpoint)));

        Middleware = new RoutingMiddleware(new[] { apiEndpoint }, staticEndpoint, GetLogger(typeof(RoutingMiddleware)));
        var wrapper = new TelegramBotApiWrapper(GetLogger(typeof(TelegramBotApiWrapper)));
        var executor = new CommandsExecutor(ControlProviders, GetLogger(typeof(CommandsExecutor)));

        BotListener = new ActiveBotListener(wrapper, executor, GetLogger(typeof(ActiveBotListener)));
    }
}