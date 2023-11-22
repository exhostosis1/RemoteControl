using ApiControllers;
using ControlProviders;
using Listeners;
using Logging;
using Servers;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.AutoStart;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
using Shared.ControlProviders.Provider;
using Shared.DIContainer.Interfaces;
using Shared.Enums;
using Shared.Listener;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;
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
#if DEBUG
            .Register<ILogger>(new TraceLogger(new TraceWrapper()))
#else
            .Register<ILogger>(new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log")))
#endif
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

    public AppBuilder RegisterConfig<T>() where T : IConfigProvider => RegisterDependency<IConfigProvider, T>(Lifetime.Singleton);

    public AppBuilder RegisterAutoStart<T>() where T : IAutoStartService =>
        RegisterDependency<IAutoStartService, T>(Lifetime.Singleton);

    public AppBuilder RegisterUserInterface<T>() where T : IUserInterface =>
        RegisterDependency<IUserInterface, T>(Lifetime.Singleton);

    public AppBuilder RegisterInputs<TKeyboard, TMouse, TDisplay, TAudio>() 
        where TKeyboard : IKeyboardInput
        where TMouse : IMouseInput
        where TDisplay : IDisplayInput
        where TAudio : IAudioInput
    {
        RegisterDependency<IKeyboardInput, TKeyboard>(Lifetime.Singleton);
        RegisterDependency<IMouseInput, TMouse>(Lifetime.Singleton);
        RegisterDependency<IDisplayInput, TDisplay>(Lifetime.Singleton);
        RegisterDependency<IAudioInput, TAudio>(Lifetime.Singleton);

        return this;
    }


    public App Build()
    {
        return new App(_containerBuilder.Build());
    }
}