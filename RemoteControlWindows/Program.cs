using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RemoteControlWindows
{
    public static class Program
    {
        public static void Main()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

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

                Process.Start(new ProcessStartInfo("netsh", args) { Verb = "runas", UseShellExecute = true, CreateNoWindow = true});
                RemoteControlMain.Main.Run(container);
            }
        }
    }
}
