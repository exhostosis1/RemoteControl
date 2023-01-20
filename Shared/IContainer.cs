using Shared.ApiControllers;
using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Collections.Generic;

namespace Shared;

public interface IContainer: IPlatformDependantContainer
{
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
    public IListener<HttpContext> NewHttpListener(IHttpListenerWrapper wrapper, ILogger logger);
    public IListener<BotContext> NewBotListener(IActiveApiWrapper wrapper, ILogger logger);
    public IActiveApiWrapper NewBotWrapper(ILogger logger);
    public IHttpListenerWrapper NewHttpWrapper();
    public ICommandExecutor NewExecutor(IControlProvider facade, ILogger logger);
    public IApiController NewAudioController(IControlProvider provider, ILogger logger);
    public IApiController NewKeyboardController(IControlProvider provider, ILogger logger);
    public IApiController NewMouseController(IControlProvider provider, ILogger logger);
    public IApiController NewDisplayController(IControlProvider provider, ILogger logger);
    public AbstractMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger, AbstractMiddleware<HttpContext>? next = null);
    public AbstractMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www");
}