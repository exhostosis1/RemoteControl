using CommunityToolkit.Mvvm.Input;
using Servers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WinUI;

internal partial class Utils
{
    [RelayCommand]
    private static void OpenSite(string input)
    {
        var address = input.Replace("&", "^&");

        RunSystemCommand($"start {address}");
    }

    [RelayCommand]
    private static void AddFirewallRule(IEnumerable<Uri> uris)
    {
        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        var translatedValue = sid.Translate(typeof(NTAccount)).Value;

        foreach (var uri in uris)
        {
            var command =
                $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

            RunSystemCommand(command, true);

            command = $"netsh http add urlacl url={uri} user={translatedValue}";

            RunSystemCommand(command, true);
        }
    }


    [RelayCommand]
    private static void Exit()
    {
        Environment.Exit(0);
    }

    private static void RunSystemCommand(string command, bool elevated = false)
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
}