using MainApp.Interfaces;
using MainApp.Servers;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;

[assembly: InternalsVisibleTo("UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace MainApp;

public sealed class AppHost: IDisposable
{
    private readonly ILogger _logger;
    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigurationProvider _configProvider;
    private readonly ServerFactory _serverFactory;

    public readonly ObservableCollection<IServer> Servers = [];

    public event EventHandler<string>? Error;

    public bool IsAutorun
    {
        get => _autoStartService.CheckAutoStart();
        set => _autoStartService.SetAutoStart(value);
    }

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

        try
        {
            foreach (var server in Servers.Where(x => x.Config.AutoStart))
            {
                server.Start();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{message}", e.Message);
            FireErrorEvent(e.Message);
        }
    }
    #endregion

    #region Public methods
    public Guid AddServer(ServerType type)
    {
        var s = _serverFactory.GetServer(new ServerConfig(type));
        Servers.Add(s);

        return s.Id;
    }

    public void RemoveServer(Guid id)
    {
        try
        {
            var s = Servers.First(x => x.Id == id);
            s.Stop();

            Servers.Remove(s);
        }
        catch (Exception e)
        {
            _logger.LogError("{message}", e.Message);
            FireErrorEvent(e.Message);
        }
        
    }

    private void RunWindowsCommand(string command, bool elevated = false)
    {
        var proc = new Process();

        proc.StartInfo.FileName = "cmd";
        proc.StartInfo.Arguments = $"/c \"{command}\"";
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.UseShellExecute = elevated;

        if (elevated) proc.StartInfo.Verb = "runas";

        try
        {
            proc.Start();

            proc.WaitForExit();
        }
        catch (Exception e)
        {
            _logger.LogError("{message}", e.Message);
            FireErrorEvent(e.Message);
        }
    }

    public void AddFirewallRules()
    {
        var uris = Servers.Where(x => x is { Config.Type: ServerType.Web })
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
        try
        {
            _configProvider.SetConfig(Servers.Select(x => x.Config));
        }
        catch (Exception e)
        {
            _logger.LogError("{message}", e.Message);
            FireErrorEvent(e.Message);
        }
    }

    public void ReloadServers()
    {
        try
        {
            if (Servers.Count > 0)
            {
                foreach (var server in Servers.Where(x => x.Status))
                {
                    server.Stop();
                }

                Servers.Clear();
            }

            foreach (var config in _configProvider.GetConfig())
            {
                Servers.Add(_serverFactory.GetServer(config));
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{message}", e.Message);
            FireErrorEvent(e.Message);
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

    public void FireErrorEvent(string message) => Error?.Invoke(this, message);
    #endregion

    #region Private methods
    private void SetSystemEvents()
    {

        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    private List<IServer> _runningServers = [];

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