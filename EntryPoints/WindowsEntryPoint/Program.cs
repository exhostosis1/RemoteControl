using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Shared.Config;
using Shared.Enums;
using Shared.Server;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

#if DEBUG
using Microsoft.Extensions.Logging.Debug;
#else
using NReco.Logging.File;
#endif


namespace WindowsEntryPoint;

public class Main
{
    private int _id = 0;
    private List<int> _ids = [];
    private readonly List<IServer> _servers;
    private readonly ILogger _logger;
    private readonly ServerFactory _serverFactory;
    private readonly RegistryAutoStartService _autoStartService;

    public event EventHandler<IServer>? ServerAdded;
    public event EventHandler<bool>? AutostartChanged;
    public event EventHandler<List<IServer>>? ServersReady;

    public Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

#if DEBUG
        var loggingProvider = new DebugLoggerProvider();
#else
        var loggingProvider = new FileLoggerProvider(Path.Combine(Environment.CurrentDirectory, "error.log"), new FileLoggerOptions
        {
            Append = true,
            MinLevel = LogLevel.Error
        });
#endif

        _autoStartService = new RegistryAutoStartService(loggingProvider.CreateLogger(nameof(RegistryAutoStartService)));

        _serverFactory = new ServerFactory(loggingProvider);

        var c = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            .Build();

        var serverConfigs = c.GetSection("ServerConfigs").GetChildren();

        var servers = new List<CommonConfig>();

        foreach (var configurationSection in serverConfigs)
        {
            switch (configurationSection["$type"])
            {
                case "web":
                    servers.Add(new WebConfig
                    {
                        AutoStart = bool.Parse(configurationSection["AutoStart"] ?? "false"),
                        Host = configurationSection["Host"] ?? "localhost",
                        Name = configurationSection["Name"] ?? "Name",
                        Port = int.Parse(configurationSection["Port"] ?? "80"),
                        Scheme = configurationSection["Scheme"] ?? "http"
                    });
                    break;
                case "bot":
                    servers.Add(new BotConfig
                    {
                        AutoStart = bool.Parse(configurationSection["AutoStart"] ?? "false"),
                        ApiKey = configurationSection["ApiKey"] ?? "",
                        ApiUri = configurationSection["ApiUrl"] ?? "",
                        Name = configurationSection["Name"] ?? "Name",
                        Usernames = configurationSection.GetSection("Usernames").GetChildren().Select(x => x.Value ?? "").ToList() ?? []
                    });
                    break;
                default:
                    break;
            }
        }

        _servers = servers.Select<CommonConfig, IServer>(x =>
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

        _logger = loggingProvider.CreateLogger(nameof(Main));
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

 //       _configProvider.SetConfig(GetConfig(_servers));
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
 //       _configProvider.SetConfig(config);
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
}
