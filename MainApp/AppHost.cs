using MainApp.Interfaces;
using MainApp.Servers;
using MainApp.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

namespace MainApp;

public sealed class AppHost
{
    private readonly ILogger _logger;
    public ServerCollectionViewModel ServerCollectionViewModel { get; init; }

    internal readonly ServerFactory ServerFactory;

    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigurationProvider _configProvider;

    internal AppHost(ILoggerProvider loggerProvider, ServerFactory serverFactory,
        RegistryAutoStartService autoStartService, IConfigurationProvider configProvider)
    {
        _logger = loggerProvider.CreateLogger(nameof(AppHost));
        ServerFactory = serverFactory;
        _autoStartService = autoStartService;
        _configProvider = configProvider;

        ServerCollectionViewModel = new(this);

        SetSystemEvents();
    }

    internal bool GetAutorun() => _autoStartService.CheckAutoStart();
    internal bool SetAutorun(bool value) => _autoStartService.SetAutoStart(value);

    private static void RunWindowsCommand(string command, bool elevated = false)
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

    internal static void AddFirewallRules(IEnumerable<Uri> uris)
    {
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

    internal void SaveConfig(IEnumerable<ServerConfig> configs)
    {
        _configProvider.SetConfig(configs);
    }

    internal IEnumerable<Server> GetServers()
    {
        return _configProvider.GetConfig().Select(x => ServerFactory.GetServer(x));
    }

    internal static void OpenSite(Uri uri)
    {
        var address = uri.ToString().Replace("&", "^&");

        RunWindowsCommand($"start {address}");
    }

    internal static void OpenSite(string uri)
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

    private void SetSystemEvents()
    {
        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    private List<ServerViewModel> _runningServers = [];

    private void SessionSwitchHandler(object sender, SessionSwitchEventArgs args)
    {
        switch (args.Reason)
        {
            case SessionSwitchReason.SessionLock:
            {
                _logger.LogInformation("Stopping servers due to logout");

                _runningServers = ServerCollectionViewModel.Servers.Where(x => x.Status).ToList();
                _runningServers.ForEach(x => x.Status = false);

                break;
            }
            case SessionSwitchReason.SessionUnlock:
            {
                _logger.LogInformation("Restoring servers");

                _runningServers.ForEach(x => x.Status = true);
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
}