using ApiControllers;
using ControlProviders;
using Listeners;
using Servers;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.ControlProviders.Provider;
using Shared.DIContainer.Interfaces;
using Shared.Enums;
using Shared.Listener;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;

namespace RemoteControlMain;

public class AppBuilder
{
    private readonly IContainerBuilder _containerBuilder;

    public AppBuilder(IContainerBuilder containerBuilder)
    {
        _containerBuilder = containerBuilder;
    }

    public AppBuilder RegisterBasicDependencies()
    {
        _containerBuilder 
            .Register(typeof(ILogger<>), typeof(LogWrapper<>), Lifetime.Singleton)
            .Register<IGeneralControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IAudioControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IMouseControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IKeyboardControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IDisplayControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IHttpListener, HttpListenerWrapper>(Lifetime.Transient)
            .Register<IWebListener, SimpleHttpListener>(Lifetime.Transient)
            .Register<IHttpClient, HttpClientWrapper>(Lifetime.Transient)
            .Register<IBotApiProvider, TelegramBotApiProvider>(Lifetime.Transient)
            .Register<IBotListener, TelegramListener>(Lifetime.Transient)
            .Register<IBotMiddleware, CommandsExecutor>(Lifetime.Singleton)
            .Register<IApiController, AudioController>(Lifetime.Singleton)
            .Register<IApiController, DisplayController>(Lifetime.Singleton)
            .Register<IApiController, MouseController>(Lifetime.Singleton)
            .Register<IApiController, KeyboardController>(Lifetime.Singleton)
            .Register<IWebMiddleware, ApiV1Middleware>(Lifetime.Singleton)
            .Register<IWebMiddleware, StaticFilesMiddleware>(Lifetime.Singleton)
            .Register<IWebMiddlewareChain, WebMiddlewareChain>(Lifetime.Singleton)
            .Register<IBotMiddlewareChain, BotMiddlewareChain>(Lifetime.Singleton)
            .Register<SimpleServer, SimpleServer>(Lifetime.Transient)
            .Register<BotServer, BotServer>(Lifetime.Transient);

        return this;
    }

    public AppBuilder RegisterDependency<TIn, TOut>(Lifetime lifetime) where TIn: class where TOut : TIn
    {
        _containerBuilder.Register<TIn, TOut>(lifetime);
        return this;
    }

    public AppBuilder RegisterDependency<T>(T obj) where T: class
    {
        _containerBuilder.Register<T>(obj);
        return this;
    }


    public App Build()
    {
        return new App(_containerBuilder.Build());
    }
}