using System;
using Shared.ApiControllers;
using Shared.ControlProviders;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Collections.Generic;
using Shared.DataObjects.Http;

namespace Shared;

public interface IContainer: IPlatformDependantContainer
{
    public IHttpListener HttpListener { get; }
    public IBotListener BotListener { get; }
    public IHttpListenerWrapper HttpWrapper { get; }
    public IActiveApiWrapper ActiveBotWrapper { get; }
    public IMiddleware Middleware { get; }
    public ICommandExecutor Executor { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public IEndpoint ApiEndpoint { get; }
    public IEndpoint StaticEndpoint { get; }
    public IHttpListener NewHttpListener(IHttpListenerWrapper wrapper, ILogger logger);
    public IBotListener NewBotListener(IActiveApiWrapper wrapper, ILogger logger);
    public IActiveApiWrapper NewBotWrapper(ILogger logger);
    public IHttpListenerWrapper NewHttpWrapper();
    public ICommandExecutor NewExecutor(ControlFacade facade, ILogger logger);
    public IApiController NewAudioController(IAudioControlProvider provider, ILogger logger);
    public IApiController NewKeyboardController(IKeyboardControlProvider provider, ILogger logger);
    public IApiController NewMouseController(IMouseControlProvider provider, ILogger logger);
    public IApiController NewDisplayController(IDisplayControlProvider provider, ILogger logger);
    public IEndpoint NewApiEndpoint(IEnumerable<IApiController> controllers, ILogger logger);
    public IEndpoint NewStaticEndpoint(ILogger logger, string directory = "www");
    public IMiddleware NewMiddleware(IEnumerable<IEndpoint> endpoints, ILogger logger, EventHandler<Context>? next = null);
}