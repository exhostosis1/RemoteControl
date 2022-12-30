using System.Runtime.InteropServices;
using Microsoft.Win32;
using Shared.Enums;

namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var processorNames = Enumerable.Empty<string>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    processorNames = RemoteControlMain.Main.ControlProcessors
                        .Where(x => x.Status == ControlProcessorStatus.Working).Select(x => x.Name);

                    foreach (var containerControlProcessor in RemoteControlMain.Main.ControlProcessors)
                    {
                        containerControlProcessor.Stop();
                    }

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    foreach (var containerControlProcessor in RemoteControlMain.Main.ControlProcessors.Where(x => processorNames.Contains(x.Name)))
                    {
                        containerControlProcessor.Start();
                    }
                    break;
                }
                default:
                    break;
            }
        };

        var container = new RemoteControlContainer();

        try
        {
            RemoteControlMain.Main.Run(container);
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }
    }
}
