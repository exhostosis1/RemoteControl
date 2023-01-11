using Shared.ApiControllers;
using Shared.ControlProviders;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Collections.Generic;

namespace Shared;

public interface IContainer: IPlatformDependantContainer
{
    public IHttpListener HttpListener { get; }
    public IBotListener BotListener { get; }
    public IHttpListenerWrapper HttpWrapper { get; }
    public IActiveApiWrapper ActiveBotWrapper { get; }
    public AbstractMiddleware Middleware { get; }
    public ICommandExecutor Executor { get; }
    public BaseApiController AudioController { get; }
    public BaseApiController MouseController { get; }
    public BaseApiController KeyboardController { get; }
    public BaseApiController DisplayController { get; }
    public AbstractApiEndpoint ApiEndpoint { get; }
    public AbstractEndpoint StaticEndpoint { get; }
    public IHttpListener NewHttpListener(IHttpListenerWrapper wrapper, ILogger logger);
    public IBotListener NewBotListener(IActiveApiWrapper wrapper, ILogger logger);
    public IActiveApiWrapper NewBotWrapper(ILogger logger);
    public IHttpListenerWrapper NewHttpWrapper();
    public ICommandExecutor NewExecutor(ControlFacade facade, ILogger logger);
    public BaseApiController NewAudioController(IAudioControlProvider provider, ILogger logger);
    public BaseApiController NewKeyboardController(IKeyboardControlProvider provider, ILogger logger);
    public BaseApiController NewMouseController(IMouseControlProvider provider, ILogger logger);
    public BaseApiController NewDisplayController(IDisplayControlProvider provider, ILogger logger);
    public AbstractApiEndpoint NewApiEndpoint(IEnumerable<BaseApiController> controllers, ILogger logger);
    public AbstractEndpoint NewStaticEndpoint(ILogger logger, string directory = "www");
    public AbstractMiddleware NewMiddleware(IEnumerable<AbstractApiEndpoint> apiEndpoints,
        AbstractEndpoint staticEndpoint, ILogger logger, HttpEventHandler? next = null);
}