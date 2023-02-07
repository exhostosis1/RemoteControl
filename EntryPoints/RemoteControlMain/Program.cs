using ApiControllers;
using ControlProviders;
using Listeners;
using Servers;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Logging;
using Shared.Server;
using Shared.UI;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;
using Shared.ControlProviders.Provider;
using Shared.DIContainer;
using Shared.Autostart;
using Shared.Listener;

namespace RemoteControlMain;

public static class Program
{
    private static int _id;
    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public static List<IServer> Servers { get; private set; } = new();

    public static void Run(ContainerBuilder inner)
    {
        var container = inner
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
            .Register<BotServer, BotServer>(Lifetime.Transient)
            .Build();

        var ui = container.GetObject<IUserInterface>();
        var configProvider = container.GetObject<IConfigProvider>();
        var autostartService = container.GetObject<IAutostartService>();
        var config = container.GetObject<IConfigProvider>().GetConfig();

        Servers = config.ServerConfigs.Select<CommonConfig, IServer>(x =>
        {
            switch (x)
            {
                case WebConfig s:
                    var server = container.GetObject<SimpleServer>();
                    server.CurrentConfig = s;
                    server.Id = _id++;
                    return server;
                case BotConfig b:
                    var bot = container.GetObject<BotServer>();
                    bot.CurrentConfig = b;
                    bot.Id = _id++;
                    return bot;
                default:
                    throw new NotSupportedException("Config not supported");
            }
        }).ToList();

        Servers.ForEach(x =>
        {
            if (x.Config.Autostart)
                x.Start();
        });

        ui.SetAutostartValue(autostartService.CheckAutostart());

        ui.OnStart += (_, id) =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Start());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Start();
            }
        };

        ui.OnStop += (_, id) =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Stop());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Stop();
            }
        };

        ui.OnServerAdded += (_, mode) =>
        {
            IServer server = mode switch
            {
                ServerType.Http => container.GetObject<SimpleServer>(),
                ServerType.Bot => container.GetObject<BotServer>(),
                _ => throw new NotSupportedException()
            };

            server.Id = _id++;

            Servers.Add(server);
            ui.AddServer(server);
        };

        ui.OnServerRemoved += (_, id) =>
        {
            var server = Servers.FirstOrDefault(x => x.Id == id);
            if (server == null)
                return;

            server.Stop();
            Servers.Remove(server);

            configProvider.SetConfig(GetConfig(Servers));
        };

        ui.OnAutostartChanged += (_, value) =>
        {
            autostartService.SetAutostart(value);
            ui.SetAutostartValue(autostartService.CheckAutostart());
        };

        ui.OnConfigChanged += (_, configTuple) =>
        {
            var server = Servers.FirstOrDefault(x => x.Id == configTuple.Item1);
            if (server == null)
                return;

            if (server.Status.Working)
            {
                server.Restart(configTuple.Item2);
            }
            else
            {
                server.Config = configTuple.Item2;
            }

            config = GetConfig(Servers);
            configProvider.SetConfig(config);
        };

        ui.OnClose += (_, _) =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(Servers);
    }
}