using Servers;
using Shared.Autostart;
using Shared.Config;
using Shared.DIContainer.Interfaces;
using Shared.Enums;
using Shared.Server;
using Shared.UI;

namespace RemoteControlMain;

public class App
{
    private int _id;
    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public List<IServer> Servers { get; set; } = new();

    private readonly ISimpleContainer _container;

    public App(ISimpleContainer container)
    {
        _container = container;
    }

    public void Run()
    {
        var ui = _container.GetObject<IUserInterface>();
        var configProvider = _container.GetObject<IConfigProvider>();
        var autostartService = _container.GetObject<IAutostartService>();
        var config = configProvider.GetConfig();

        Servers = config.ServerConfigs.Select<CommonConfig, IServer>(x =>
        {
            switch (x)
            {
                case WebConfig s:
                    var server = _container.GetObject<SimpleServer>();
                    server.CurrentConfig = s;
                    server.Id = _id++;
                    return server;
                case BotConfig b:
                    var bot = _container.GetObject<BotServer>();
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
                ServerType.Http => _container.GetObject<SimpleServer>(),
                ServerType.Bot => _container.GetObject<BotServer>(),
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