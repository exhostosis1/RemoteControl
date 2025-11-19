using MainApp.Interfaces;
using MainApp.Servers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Principal;

namespace MainApp;

public sealed class AppHost
{
    private readonly ILogger _logger;

    public readonly ServerFactory ServerFactory;

    private readonly RegistryAutoStartService _autoStartService;
    private readonly IConfigurationProvider _configProvider;

    internal AppHost(ILoggerProvider loggerProvider, ServerFactory serverFactory,
        RegistryAutoStartService autoStartService, IConfigurationProvider configProvider)
    {
        _logger = loggerProvider.CreateLogger(nameof(AppHost));
        ServerFactory = serverFactory;
        _autoStartService = autoStartService;
        _configProvider = configProvider;
    }

    public bool GetAutorun() => _autoStartService.CheckAutoStart();
    public bool SetAutorun(bool value) => _autoStartService.SetAutoStart(value);

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

    public void AddFirewallRules(IEnumerable<Uri> uris)
    {
        _logger.LogWarning("Adding firewall rules");

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

    public void AddListeningPermissionsToUser(IEnumerable<string> uris)
    {
        _logger.LogWarning("Trying to add listening permissions to user");

        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        var translatedValue = sid.Translate(typeof(NTAccount)).Value;

        foreach (var uri in uris)
        {
            var command = $"netsh http add urlacl url={uri} user={translatedValue}";

            RunWindowsCommand(command, true);
        }
    }

    public void SaveConfig(IEnumerable<ServerConfig> configs)
    {
        _configProvider.SetConfig(configs);
    }

    public IEnumerable<IWorker> GetWorkers()
    {
        return _configProvider.GetConfig().Select(x => ServerFactory.GetServer(x));
    }

    public static void OpenSite(Uri uri)
    {
        var address = uri.ToString().Replace("&", "^&");

        RunWindowsCommand($"start {address}");
    }

    public static void OpenSite(string uri)
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

    public ILogger GetLogger() => _logger;
}