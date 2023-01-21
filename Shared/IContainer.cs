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
    public TelegramBotApiProvider TelegramBotApiProvider { get; }
    public IApiController AudioController { get; }
    public IApiController MouseController { get; }
    public IApiController KeyboardController { get; }
    public IApiController DisplayController { get; }
    public AbstractMiddleware<HttpContext> ApiMiddleware { get; }
    public AbstractMiddleware<HttpContext> StaticMiddleware { get; }
    public AbstractMiddleware<BotContext> CommandExecutor { get; }

    public IListener<HttpContext> NewWebListener(IHttpListener listener);
    public IListener<BotContext> NewBotListener(TelegramBotApiProvider provider, ILogger logger);
    public IHttpClient NewHttpClient();
    public IHttpListener NewHttpListener();
    public TelegramBotApiProvider NewTelegramBotApiProvider(IHttpClient client, ILogger logger);
    public IApiController NewAudioController(IControlProvider provider, ILogger logger);
    public IApiController NewKeyboardController(IControlProvider provider, ILogger logger);
    public IApiController NewMouseController(IControlProvider provider, ILogger logger);
    public IApiController NewDisplayController(IControlProvider provider, ILogger logger);
    public AbstractMiddleware<HttpContext> NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger, AbstractMiddleware<HttpContext>? next = null);
    public AbstractMiddleware<HttpContext> NewStaticMiddleware(ILogger logger, string directory = "www");
    public AbstractMiddleware<BotContext> NewCommmandExecutor(IControlProvider provider, ILogger logger);
}