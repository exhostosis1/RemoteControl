using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

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
                var args =
                    $"http add urlacl url={container.Config.GetConfig().UriConfig.Uri} user={System.Security.Principal.WindowsIdentity.GetCurrent().Name}";

                Process.Start(new ProcessStartInfo("netsh", args) { Verb = "runas", UseShellExecute = true});
                RemoteControlMain.Main.Run(container);
            }
        }
    }
}
