using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Servers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Principal;

namespace MainApp;

public sealed class AppHost: IDisposable
{
    private readonly ILogger _logger;
    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigurationProvider _configProvider;

    public ServerFactory ServerFactory { get; init; }
    public readonly ObservableCollection<Server> Servers = [];


    private bool? _isAutostartCached = null;
    public bool IsAutostart
    {
        get => _isAutostartCached ??= _autoStartService.CheckAutoStart();
        set
        {
            _autoStartService.SetAutoStart(value);
            _isAutostartCached = null;
        }
    }

    #region Constructor
    internal AppHost(ILoggerProvider loggerProvider, ServerFactory serverFactory,
        RegistryAutoStartService autoStartService, IConfigurationProvider configProvider)
    {
        _logger = loggerProvider.CreateLogger(nameof(AppHost));
        ServerFactory = serverFactory;
        _autoStartService = autoStartService;
        _configProvider = configProvider;

        ReloadServers();
        SetSystemEvents();
    }
    #endregion

    private IEnumerable<Server> GenerateServers(IEnumerable<ServerConfig> configs)
    {
        foreach (var serverConfig in configs)
        {
            yield return serverConfig.Type switch
            {
                ServerType.Web => ServerFactory.GetServer(serverConfig),
                ServerType.Bot => ServerFactory.GetBot(serverConfig),
                _ => throw new NotSupportedException("Config not supported")
            };
        };
    }

    #region Public methods
    public void RunAll()
    {
        try
        {
            foreach (var server in Servers.Where(x => x.Config.AutoStart))
            {
                server.Start();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{e.Message}", e.Message);
        }
    }

    public static void RunCommand(string command, bool elevated = false)
    {
        var proc = new Process();

        proc.StartInfo.FileName = "cmd";
        proc.StartInfo.Arguments = $"/c \"{command}\"";
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.UseShellExecute = elevated;

        if (elevated) proc.StartInfo.Verb = "runas";

        proc.Start();

        proc.WaitForExit();
    }

    public void AddFirewallRules()
    {
        var uris = Servers.Where(x => x is { Type: ServerType.Web })
            .Select(x => x.Config.Uri);

        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        var translatedValue = sid.Translate(typeof(NTAccount)).Value;

        foreach (var uri in uris)
        {
            var command =
                $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

            RunCommand(command, true);

            command = $"netsh http add urlacl url={uri} user={translatedValue}";

            RunCommand(command, true);
        }
    }

    public void SaveConfig()
    {
        _configProvider.SetConfig(Servers.Select(x => x.Config));
    }

    public void ReloadServers()
    {
        if (Servers.Count > 0)
        {
            foreach (var server in Servers.Where(x => x.Status))
            {
                server.Stop();
            }

            Servers.Clear();
        }

        foreach (var server in GenerateServers(_configProvider.GetConfig()))
        {
            Servers.Add(server);
        }
    }
    #endregion

    #region Private methods
    private void SetSystemEvents()
    {

        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    private List<Server> _runningServers = [];

    private void SessionSwitchHandler(object sender, SessionSwitchEventArgs args)
    {
        switch (args.Reason)
        {
            case SessionSwitchReason.SessionLock:
            {
                _logger.LogInformation("Stopping servers due to logout");

                _runningServers = Servers.Where(x => x.Status).ToList();
                _runningServers.ForEach(x => x.Stop());

                break;
            }
            case SessionSwitchReason.SessionUnlock:
            {
                _logger.LogInformation("Restoring servers");

                _runningServers.ForEach(x => x.Start());
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
    }
    #endregion

    public void Dispose()
    {
        SystemEvents.SessionSwitch -= SessionSwitchHandler;
        foreach (var server in Servers)
        {
            server.Stop();
        }

        Servers.Clear();
    }
}