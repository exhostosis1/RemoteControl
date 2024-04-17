using MainApp.DTO;
using MainApp.Interfaces;
using MainApp.Servers;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;

namespace MainApp;

public sealed class AppHost: IDisposable
{
    private readonly ILogger _logger;
    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigurationProvider _configProvider;

    private readonly ServerFactory _serverFactory;
    private readonly List<Server> _servers = [];

    private bool IsAutostart
    {
        get => _autoStartService.CheckAutoStart();
        set => _autoStartService.SetAutoStart(value);
    }

    public event EventHandler<(Guid, bool)>? ServerStatusChanged; 

    #region Constructor
    internal AppHost(ILoggerProvider loggerProvider, ServerFactory serverFactory,
        RegistryAutoStartService autoStartService, IConfigurationProvider configProvider)
    {
        _logger = loggerProvider.CreateLogger(nameof(AppHost));
        _serverFactory = serverFactory;
        _autoStartService = autoStartService;
        _configProvider = configProvider;

        ReloadServers();
        SetSystemEvents();

        foreach (var server in _servers.Where(x => x.Config.AutoStart))
        {
            server.Start();
        }
    }
    #endregion

    #region Public methods

    public void StartAllServers()
    {
        foreach (var server in _servers)
        {
            server.Start();
        }
    }

    public void StopAllServers()
    {
        foreach (var server in _servers)
        {
            server.Stop();
        }
    }

    public bool StartServer(Guid id)
    {
        var s = _servers.First(x => x.Id == id);

        s.Start();
        return s.Status;
    }

    public bool StopServer(Guid id)
    {
        var s = _servers.First(x => x.Id == id);

        s.Stop();
        return s.Status;
    }

    public void SetConfig(Guid id, ServerConfig config)
    {
        var s = _servers.First(x => x.Id == id);

        s.Config = config;
    }

    public Guid AddServer(ServerType type)
    {
        var s = CreateServer(new ServerConfig(type));
        _servers.Add(s);

        return s.Id;
    }

    public void RemoveServer(Guid id)
    {
        var s = _servers.First(x => x.Id == id);

        s.PropertyChanged -= ServerStatusChangedHandler;
        s.Stop();

        _servers.Remove(s);
    }

    public bool GetAutostart() => IsAutostart;
    public void SetAutostart(bool value) => IsAutostart = value;

    public static void RunWindowsCommand(string command, bool elevated = false)
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
        var uris = _servers.Where(x => x is { Config.Type: ServerType.Web })
            .Select(x => x.Config.Uri);

        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        var translatedValue = sid.Translate(typeof(NTAccount)).Value;

        foreach (var uri in uris)
        {
            var command =
                $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

            RunWindowsCommand(command, true);

            command = $"netsh http add urlacl url={uri} user={translatedValue}";

            RunWindowsCommand(command, true);
        }
    }

    public void SaveConfig()
    {
        _configProvider.SetConfig(_servers.Select(x => x.Config));
    }

    public void ReloadServers()
    {
        if (_servers.Count > 0)
        {
            foreach (var server in _servers.Where(x => x.Status))
            {
                server.Stop();
            }

            _servers.Clear();
        }

        foreach (var config in _configProvider.GetConfig())
        {
            _servers.Add(CreateServer(config));
        }
    }

    public void OpenSite(Uri uri)
    {
        var address = uri.ToString().Replace("&", "^&");

        RunWindowsCommand($"start {address}");
    }

    public void OpenSite(string uri)
    {
        Uri url;
        try
        {
            url = new Uri(uri);
        }
        catch
        {
            return;
        }

        OpenSite(url);
    }

    public IEnumerable<IServer> GetServers()
    {
        return _servers.Select<Server, IServer>(server =>
        {
            return server.Config.Type switch
            {
                ServerType.Web => new WebServerDto
                {
                    Id = server.Id,
                    Name = server.Config.Name,
                    IsAutostart = server.Config.AutoStart,
                    ListeningUri = server.Config.Uri,
                    Status = server.Status
                },
                ServerType.Bot => new BotServerDto
                {
                    Id = server.Id,
                    Name = server.Config.Name,
                    IsAutostart = server.Config.AutoStart,
                    ApiUri = new Uri(server.Config.ApiUri),
                    ApiKey = server.Config.ApiKey,
                    Status = server.Status,
                    Usernames = [.. server.Config.Usernames]
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        });
    }
    #endregion

    #region Private methods

    private Server CreateServer(ServerConfig config)
    {
        var result = _serverFactory.GetServer(config);
        result.PropertyChanged += ServerStatusChangedHandler;

        return result;
    }

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

                _runningServers = _servers.Where(x => x.Status).ToList();
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

    private void ServerStatusChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (sender is not Server server || args.PropertyName != nameof(server.Status)) return;

        ServerStatusChanged?.Invoke(this, (server.Id, server.Status));
    }
    #endregion

    public void Dispose()
    {
        SystemEvents.SessionSwitch -= SessionSwitchHandler;
        foreach (var server in _servers)
        {
            server.Stop();
            server.PropertyChanged -= ServerStatusChangedHandler;
        }

        _servers.Clear();
    }
}