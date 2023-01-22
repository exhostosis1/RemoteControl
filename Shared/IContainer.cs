using Shared.ApiControllers;
using Shared.Bots.Telegram;
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

    public IListener<HttpContext> NewWebListener(IHttpListener listener, ILogger logger);
    public IListener<BotContext> NewBotListener(IBotApiProvider provider, ILogger logger);
    public IHttpClient NewHttpClient();
    public IHttpListener NewHttpListener(ILogger logger);
    public IBotApiProvider NewTelegramBotApiProvider(IHttpClient client, ILogger logger);
    public IApiController NewAudioController(IControlProvider provider, ILogger logger);
    public IApiController NewKeyboardController(IControlProvider provider, ILogger logger);
    public IApiController NewMouseController(IControlProvider provider, ILogger logger);
    public IApiController NewDisplayController(IControlProvider provider, ILogger logger);
    public IMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger, IMiddleware<HttpContext>? next = null);
    public IMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www");
    public IMiddleware<BotContext> NewCommmandExecutor(IControlProvider provider, ILogger logger);
}