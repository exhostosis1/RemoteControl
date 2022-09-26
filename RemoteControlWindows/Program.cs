using Shared;
using System.Net;
using System.Runtime.InteropServices;
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

            var command =
                $"netsh http add urlacl url={container.Config.GetConfig().UriConfig.Uri} user={translatedValue}";

            Utils.RunCommand(OSPlatform.Windows, command, out _, out _, true, false, true);

            RemoteControlMain.Main.Run(container);
        }
    }
}
