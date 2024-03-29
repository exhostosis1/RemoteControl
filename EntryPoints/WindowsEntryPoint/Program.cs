using AutoStart;
using ConfigProviders;
using Logging;
using Microsoft.Win32;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Wrappers.Registry;
using System.Runtime.InteropServices;

namespace WindowsEntryPoint;

public class Main
{
    private int _id = 0;
    private List<int> _ids = [];
    private readonly List<IServer> _servers;
    private readonly ILogger<Main> _logger;
    private readonly ServerFactory _serverFactory;
    private readonly LocalFileConfigProvider _configProvider;
    private readonly RegistryAutoStartService _autoStartService;

    public event EventHandler<IServer>? ServerAdded;
    public event EventHandler<bool>? AutostartChanged;
    public event EventHandler<List<IServer>>? ServersReady;

    public Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

#if DEBUG
        var logger = new TraceLogger(new TraceWrapper());
#else
        var logger = new FileLogger("error.log");
#endif

        _configProvider = new LocalFileConfigProvider(new LogWrapper<LocalFileConfigProvider>(logger), Path.Combine(Environment.CurrentDirectory, "config.ini"));
        _autoStartService =
            new RegistryAutoStartService(new RegistryWrapper(), new LogWrapper<RegistryAutoStartService>(logger));

        _serverFactory = new ServerFactory(logger);

        var config = _configProvider.GetConfig();

        _servers = config.ServerConfigs.Select<CommonConfig, IServer>(x =>
        {
            switch (x)
            {
                case WebConfig s:
                    var server = _serverFactory.GetServer();
                    server.CurrentConfig = s;
                    server.Id = _id++;
                    return server;
                case BotConfig b:
                    var bot = _serverFactory.GetBot();
                    bot.CurrentConfig = b;
                    bot.Id = _id++;
                    return bot;
                default:
                    throw new NotSupportedException("Config not supported");
            }
        }).ToList();

        _logger = new LogWrapper<Main>(logger);
    }

    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public void ServerStart(int? id)
    {
        if (!id.HasValue)
        {
            _servers.ForEach(x => x.Start());
        }
        else
        {
            _servers.FirstOrDefault(x => x.Id == id)?.Start();
        }
    }

    public void ServerStop(int? id)
    {
        if (!id.HasValue)
        {
            _servers.ForEach(x => x.Stop());
        }
        else
        {
            _servers.FirstOrDefault(x => x.Id == id)?.Stop();
        }
    }

    public void ServerAdd(ServerType mode)
    {
        IServer server = mode switch
        {
            ServerType.Http => _serverFactory.GetServer(),
            ServerType.Bot => _serverFactory.GetBot(),
            _ => throw new NotSupportedException()
        };

        server.Id = _id++;

        _servers.Add(server);
        ServerAdded?.Invoke(this, server);
    }

    public void ServerRemove(int id)
    {
        var server = _servers.FirstOrDefault(x => x.Id == id);
        if (server == null)
            return;

        server.Stop();
        _servers.Remove(server);

        _configProvider.SetConfig(GetConfig(_servers));
    }

    public void AutoStartChange(bool value)
    {
        _autoStartService.SetAutoStart(value);
        AutostartChanged?.Invoke(this, _autoStartService.CheckAutoStart());
    }

    public void ConfigChange((int, CommonConfig) configTuple)
    {
        var server = _servers.FirstOrDefault(x => x.Id == configTuple.Item1);
        if (server == null)
            return;

        if (server.Status)
        {
            server.Restart(configTuple.Item2);
        }
        else
        {
            server.Config = configTuple.Item2;
        }

        var config = GetConfig(_servers);
        _configProvider.SetConfig(config);
    }

    public void AppClose(object? _)
    {
        Environment.Exit(0);
    }
    
    public void Run()
    {
        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    {
                        _logger.LogInfo("Stopping servers due to logout");


                        _ids = _servers.Where(x => x.Status).Select(x =>
                        {
                            x.Stop();
                            return x.Id;
                        }).ToList();

                        break;
                    }
                case SessionSwitchReason.SessionUnlock:
                    {
                        _logger.LogInfo("Restoring servers");

                        _ids.ForEach(id => _servers.Single(s => s.Id == id).Start());
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
            _servers.ForEach(x =>
            {
                if (x.Config.AutoStart)
                    x.Start();
            });

            AutostartChanged?.Invoke(this, _autoStartService.CheckAutoStart());

            ServersReady?.Invoke(this, _servers);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}
