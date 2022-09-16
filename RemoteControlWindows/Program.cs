using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();
        try
        {
            RemoteControlMain.Main.Run(container);
        }
        catch (HttpListenerException)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;

            var args =
                $"http add urlacl url={container.Config.GetConfig().UriConfig.Uri} user={translatedValue}";

            Process.Start(new ProcessStartInfo("netsh", args)
                {Verb = "runas", UseShellExecute = true, CreateNoWindow = true});
            RemoteControlMain.Main.Run(container);
        }
    }
}
