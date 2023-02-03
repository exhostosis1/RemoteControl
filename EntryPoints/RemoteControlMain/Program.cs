using System.Reflection;
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

    private static IServer CreateSimpleServer(Container container, WebConfig? config = null)
    {
        var result = new SimpleServer(container.GetObject<IWebListener>(), container.GetObject<IWebMiddleware>(),
            new LogWrapper<SimpleServer>(container.GetObject<ILogger>()), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IServer CreateTelegramBot(Container container, BotConfig? config = null)
    {
        var result = new BotServer(container.GetObject<IBotListener>(), container.GetObject<IBotMiddleware>(),
            new LogWrapper<BotServer>(container.GetObject<ILogger>()), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IEnumerable<IServer> CreateServers(AppConfig config, Container container)
    {
        return config.ServerConfigs.Select(x => x switch
        {
            WebConfig s => CreateSimpleServer(container, s),
            BotConfig b => CreateTelegramBot(container, b),
            _ => throw new NotSupportedException("Config not supported")
        });
    }

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
            .Register<IWebMiddleware>(apiMiddleWare);

        var ui = container.GetObject<IUserInterface>();
        var configProvider = container.GetObject<IConfigProvider>();
        var autostartService = container.GetObject<IAutostartService>();
        var config = container.GetObject<IConfigProvider>().GetConfig();

        Servers = CreateServers(config, container).ToList();

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
            var server = mode switch
            {
                ServerType.Http => CreateSimpleServer(container),
                ServerType.Bot => CreateTelegramBot(container),
                _ => throw new NotSupportedException()
            };

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

        ui.OnClose += (_, args) =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(Servers);
    }
}