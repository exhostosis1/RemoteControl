using Servers;
using Shared;
using Shared.Config;
using Shared.Enums;
using Shared.Logging;
using Shared.Server;

namespace RemoteControlMain;

public static class Program
{
    private static int _id;

    private static IServer CreateSimpleServer(IContainer container, WebConfig? config = null)
    {
        var result = new SimpleServer(
            container.NewWebListener(container.NewHttpListener(container.Logger), container.Logger),
            container.ApiMiddleware,
            new LogWrapper<SimpleServer>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IServer CreateTelegramBot(IContainer container, BotConfig? config = null)
    {
        var result = new BotServer(
            container.NewBotListener(container.NewTelegramBotApiProvider(container.NewHttpClient(), container.Logger),
                container.Logger), container.CommandExecutor,
            new LogWrapper<BotServer>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IEnumerable<IServer> CreateServers(AppConfig config, IContainer container)
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

    public static void Run(IPlatformDependantContainer lesserContainer)
    {
        var container = new Container(lesserContainer);

        var mutex = new Mutex(false, "RemoteControlMutex");

        try
        {
            if (mutex.WaitOne(0, false))
            {
                DoJob(container);
            }
            else
            {
                container.Logger.LogError(typeof(Program), "Another instance of the application is already running.");
                container.Logger.Flush();
                Environment.Exit(0);
            }
        }
        finally
        {
            mutex.Close();
        }
    }

    private static void DoJob(IContainer container)
    {
        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        Servers = CreateServers(config, container).ToList();

        Servers.ForEach(x =>
        {
            if (x.Config.Autostart)
                x.Start();
        });

        ui.SetAutostartValue(container.AutostartService.CheckAutostart());

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

            container.ConfigProvider.SetConfig(GetConfig(Servers));
        };

        ui.OnAutostartChanged += (_, value) =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
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
            container.ConfigProvider.SetConfig(config);
        };

        ui.OnClose += (_, args) =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(Servers);
    }
}