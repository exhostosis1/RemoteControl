using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.ControlProviders.Provider;
using Shared.DataObjects.Bot;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;
using System.Collections.Generic;
using Shared.DataObjects.Web;

namespace Shared;

public interface IContainer : IPlatformDependantContainer
{
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

    public IWebListener NewWebListener(IHttpListener listener, ILogger logger);
    public IBotListener NewBotListener(IBotApiProvider provider, ILogger logger);
    public IHttpClient NewHttpClient();
    public IHttpListener NewHttpListener(ILogger logger);
    public IBotApiProvider NewTelegramBotApiProvider(IHttpClient client, ILogger logger);
    public IApiController NewAudioController(IAudioControlProvider provider, ILogger logger);
    public IApiController NewKeyboardController(IKeyboardControlProvider provider, ILogger logger);
    public IApiController NewMouseController(IMouseControlProvider provider, ILogger logger);
    public IApiController NewDisplayController(IDisplayControlProvider provider, ILogger logger);
    public IWebMiddleware NewApiMiddleware(IEnumerable<IApiController> controllers, ILogger logger, IWebMiddleware? next = null);
    public IWebMiddleware NewStaticMiddleware(ILogger logger, string directory = "www");
    public IBotMiddleware NewCommmandExecutor(IGeneralControlProvider provider, ILogger logger);
}