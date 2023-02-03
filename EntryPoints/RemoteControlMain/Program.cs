using ApiControllers;
using Listeners;
using Servers;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers;
using Shared.Bots.Telegram;
using Shared.Config;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.UI;
using Shared.Wrappers.HttpClient;
using Shared.Wrappers.HttpListener;

namespace RemoteControlMain;

public static class Program
{
    private static int _id;
    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public static List<IServer> Servers { get; private set; } = new();

    public static void Run(Container inner)
    {
        var audioController = new AudioController(inner.GetObject<IGeneralControlProvider>(),
            new LogWrapper<AudioController>(inner.GetObject<ILogger>()));
        var dispalyController = new DisplayController(inner.GetObject<IGeneralControlProvider>(),
            new LogWrapper<DisplayController>(inner.GetObject<ILogger>()));
        var keyboardController = new KeyboardController(inner.GetObject<IGeneralControlProvider>(),
            new LogWrapper<KeyboardController>(inner.GetObject<ILogger>()));
        var mouseController = new MouseController(inner.GetObject<IGeneralControlProvider>(),
            new LogWrapper<MouseController>(inner.GetObject<ILogger>()));

        var staticMiddleware = new StaticFilesMiddleware(new LogWrapper<StaticFilesMiddleware>(inner.GetObject<ILogger>()));
        var apiMiddleWare =
            new ApiV1Middleware(new IApiController[] { audioController, keyboardController, mouseController, dispalyController },
                new LogWrapper<ApiV1Middleware>(inner.GetObject<ILogger>()), staticMiddleware);

        var container = inner
            .Register<IHttpListener, HttpListenerWrapper>(Lifetime.Transient)
            .Register<IWebListener, SimpleHttpListener>(Lifetime.Transient)
            .Register<IHttpClient, HttpClientWrapper>(Lifetime.Transient)
            .Register<IBotApiProvider, TelegramBotApiProvider>(Lifetime.Transient)
            .Register<IBotListener, TelegramListener>(Lifetime.Transient)
            .Register<IBotMiddleware, CommandsExecutor>(Lifetime.Singleton)
            .Register<IWebMiddleware>(apiMiddleWare)
            .Register<SimpleServer, SimpleServer>(Lifetime.Transient)
            .Register<BotServer, BotServer>(Lifetime.Transient);

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

        ui.SetAutostartValue(container.GetObject<IAutostartService>().CheckAutostart());

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