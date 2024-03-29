using AutoStart;
using ConfigProviders;
using Logging;
using Microsoft.Win32;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;
using Shared.Observable;
using Shared.Server;
using Shared.Wrappers.Registry;
using System.Runtime.InteropServices;
using WinFormsUI;

namespace WindowsEntryPoint;

public static class Program
{
    private static int _id;
    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

#if DEBUG
        var logger = new ConsoleLogger(new ConsoleWrapper());
#else
        var logger = new FileLogger("error.log");
#endif

        var ui = new MainForm();
        var configProvider = new LocalFileConfigProvider(new LogWrapper<LocalFileConfigProvider>(logger), "config.ini");
        var autoStartService =
            new RegistryAutoStartService(new RegistryWrapper(), new LogWrapper<RegistryAutoStartService>(logger));

        var serverFactory = new ServerFactory(logger);
        
        var type = typeof(Program);

        var ids = new List<int>();

        var config = configProvider.GetConfig();

        var servers = config.ServerConfigs.Select<CommonConfig, IServer>(x =>
        {
            switch (x)
            {
                case WebConfig s:
                    var server = serverFactory.GetServer();
                    server.CurrentConfig = s;
                    server.Id = _id++;
                    return server;
                case BotConfig b:
                    var bot = serverFactory.GetBot();
                    bot.CurrentConfig = b;
                    bot.Id = _id++;
                    return bot;
                default:
                    throw new NotSupportedException("Config not supported");
            }
        }).ToList();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    {
                        logger.Log(type, "Stopping servers due to logout", LoggingLevel.Info);

                        ids = servers.Where(x => x.Status.Working).Select(x =>
                        {
                            x.Stop();
                            return x.Id;
                        }).ToList();

                        break;
                    }
                case SessionSwitchReason.SessionUnlock:
                    {
                        logger.Log(type, "Restoring servers", LoggingLevel.Info);

                        ids.ForEach(id => servers.Single(s => s.Id == id).Start());
                        break;
                    }
                case SessionSwitchReason.ConsoleConnect:
                case SessionSwitchReason.ConsoleDisconnect:
                case SessionSwitchReason.RemoteConnect:
                case SessionSwitchReason.RemoteDisconnect:
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionLogoff:
                case SessionSwitchReason.SessionRemoteControl:
                default:
                    break;
            }
        };

        try
        {
            servers.ForEach(x =>
            {
                if (x.Config.AutoStart)
                    x.Start();
            });

            ui.SetAutoStartValue(autoStartService.CheckAutoStart());

            ui.ServerStart.Subscribe(new MyObserver<int?>(id =>
            {
                if (!id.HasValue)
                {
                    servers.ForEach(x => x.Start());
                }
                else
                {
                    servers.FirstOrDefault(x => x.Id == id)?.Start();
                }
            }));

            ui.ServerStop.Subscribe(new MyObserver<int?>(id =>
            {
                if (!id.HasValue)
                {
                    servers.ForEach(x => x.Stop());
                }
                else
                {
                    servers.FirstOrDefault(x => x.Id == id)?.Stop();
                }
            }));

            ui.ServerAdd.Subscribe(new MyObserver<ServerType>(mode =>
            {
                IServer server = mode switch
                {
                    ServerType.Http => serverFactory.GetServer(),
                    ServerType.Bot => serverFactory.GetBot(),
                    _ => throw new NotSupportedException()
                };

                server.Id = _id++;

                servers.Add(server);
                ui.AddServer(server);
            }));

            ui.ServerRemove.Subscribe(new MyObserver<int>(id =>
            {
                var server = servers.FirstOrDefault(x => x.Id == id);
                if (server == null)
                    return;

                server.Stop();
                servers.Remove(server);

                configProvider.SetConfig(GetConfig(servers));
            }));

            ui.AutoStartChange.Subscribe(new MyObserver<bool>(value =>
            {
                autoStartService.SetAutoStart(value);
                ui.SetAutoStartValue(autoStartService.CheckAutoStart());
            }));

            ui.ConfigChange.Subscribe(new MyObserver<(int, CommonConfig)>(configTuple =>
            {
                var server = servers.FirstOrDefault(x => x.Id == configTuple.Item1);
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

                config = GetConfig(servers);
                configProvider.SetConfig(config);
            }));

            ui.AppClose.Subscribe(new MyObserver<object?>(_ =>
            {
                Environment.Exit(0);
            }));

            ui.RunUI(servers);
        }
        catch (Exception e)
        {
            logger.Log(type, e.Message, LoggingLevel.Error);
        }
    }
}
