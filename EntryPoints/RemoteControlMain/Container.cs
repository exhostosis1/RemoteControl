using ApiControllers;
using Bots;
using Listeners;
using Listeners.Wrappers;
using Listeners.Wrappers.Telegram;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Listeners;
using Shared.Logging;
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
    public ICommandExecutor Executor { get; }

    public ILogger Logger { get; }

    public Container(IPlatformDependantContainer input)
    {
        ConfigProvider = input.ConfigProvider;
        AutostartService = input.AutostartService;
        UserInterface = input.UserInterface;
        ControlProviders = input.ControlProviders;
        Logger = input.Logger;

        var httpWrapper = new HttpListenerWrapper();

        HttpListener = new SimpleHttpListener(httpWrapper, new LogWrapper<SimpleHttpListener>(Logger));

        var controllers = new BaseApiController[]
        {
            new AudioController(ControlProviders.Audio, new LogWrapper<AudioController>(Logger)),
            new DisplayController(ControlProviders.Display, new LogWrapper<DisplayController>(Logger)),
            new KeyboardController(ControlProviders.Keyboard, new LogWrapper<KeyboardController>(Logger)),
            new MouseController(ControlProviders.Mouse, new LogWrapper<MouseController>(Logger))
        };
        var apiEndpoint = new ApiV1Endpoint(controllers, new LogWrapper<ApiV1Endpoint>(Logger));
        var staticEndpoint = new StaticFilesEndpoint(new LogWrapper<StaticFilesEndpoint>(Logger));

        Middleware = new RoutingMiddleware(new[] { apiEndpoint }, staticEndpoint, new LogWrapper<RoutingMiddleware>(Logger));
        var wrapper = new TelegramBotApiWrapper(new LogWrapper<TelegramBotApiWrapper>(Logger));
        Executor = new CommandsExecutor(ControlProviders, new LogWrapper<CommandsExecutor>(Logger));

        BotListener = new ActiveBotListener(wrapper,new LogWrapper<ActiveBotListener>(Logger));
    }
}