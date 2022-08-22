using Logging;
using System.Runtime.InteropServices;

namespace RemoteControl
{
    public static class Program
    {
        public static void Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var container = new RemoteControlMain();
                var logger = new ConsoleLogger();

                RemoteControlWinForms.Program.Inject(container, logger);
                RemoteControlWinForms.Program.Main();
            }
        }
    }
}
