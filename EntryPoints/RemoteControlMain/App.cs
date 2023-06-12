using Servers;
using Shared.Autostart;
using Shared.Config;
using Shared.DIContainer.Interfaces;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Observable;
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

    public ILogger Logger { get; }

    public App(ISimpleContainer container)
    {
        _container = container;
        Logger = _container.GetObject<ILogger>();
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

        ui.ServerStart.Subscribe(new MyObserver<int?>( id =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Start());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Start();
            }
        }));

        ui.ServerStop.Subscribe(new MyObserver<int?>(id =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Stop());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Stop();
            }
        }));

        ui.ServerAdd.Subscribe(new MyObserver<ServerType>(mode =>
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
        }));

        ui.ServerRemove.Subscribe(new MyObserver<int>(id =>
        {
            var server = Servers.FirstOrDefault(x => x.Id == id);
            if (server == null)
                return;

            server.Stop();
            Servers.Remove(server);

            configProvider.SetConfig(GetConfig(Servers));
        }));

        ui.AutostartChange.Subscribe(new MyObserver<bool>(value =>
        {
            autostartService.SetAutostart(value);
            ui.SetAutostartValue(autostartService.CheckAutostart());
        }));

        ui.ConfigChange.Subscribe(new MyObserver<(int, CommonConfig)>(configTuple =>
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
        }));

        ui.AppClose.Subscribe(new MyObserver<object?>(_ =>
        {
            Environment.Exit(0);
        }));

        ui.RunUI(Servers);
    }
}