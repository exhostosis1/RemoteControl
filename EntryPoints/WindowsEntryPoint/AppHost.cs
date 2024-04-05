using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Servers;
using Shared.Config;

namespace AppHost;

public  class AppHost
{
    private readonly ILogger _logger;
    private readonly ServerFactory _serverFactory;
    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigProvider _configProvider;

    private int _id = 0;
    private List<int> _ids = [];
    private readonly List<Server> _servers;


    public event EventHandler<Server>? ServerAdded;
    public event EventHandler<bool>? AutostartChanged;
    public event EventHandler<List<Server>>? ServersReady;

    #region Constructor
    internal AppHost(ILoggerProvider loggerProvider, ServerFactory serverFactory,
        RegistryAutoStartService autoStartService, IConfigProvider configProvider)
    {
        _logger = loggerProvider.CreateLogger(nameof(AppHost));
        _serverFactory = serverFactory;
        _autoStartService = autoStartService;
        _configProvider = configProvider;

        var config = _configProvider.GetConfig();

        _servers = config.ServerConfigs.Select<ServerConfig, Server>(x =>
        {
            return x.Type switch
            {
                ServerType.Web => _serverFactory.GetServer(x, _id++),
                ServerType.Bot => _serverFactory.GetBot(x, _id++),
                _ => throw new NotSupportedException("Config not supported")
            };
        }).ToList();

        SetSystemEvents();
    }
    #endregion

    #region Public methods
    public void Run()
    {
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
            _logger.LogError("{e.Message}", e.Message);
        }
    }

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
        Server server = mode switch
        {
            ServerType.Web => _serverFactory.GetServer(),
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

    public void ConfigChange((int, ServerConfig) configTuple)
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
    #endregion


    #region Private methods
    private static AppConfig GetConfig(IEnumerable<Server> servers) =>
        new(servers.Select(x => x.Config).ToList());

    private void SetSystemEvents()
    {
        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    _logger.LogInformation("Stopping servers due to logout");


                    _ids = _servers.Where(x => x.Status).Select(x =>
                    {
                        x.Stop();
                        return x.Id;
                    }).ToList();

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    _logger.LogInformation("Restoring servers");

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
    }
    #endregion
}