using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();

        var config = container.ConfigProvider.GetConfig();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    foreach (var containerControlProcessor in container.ControlProcessors)
                    {
                        containerControlProcessor.Stop();
                    }

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    foreach (var containerControlProcessor in container.ControlProcessors)
                    {
                        containerControlProcessor.Start(config);
                    }
                    break;
                }
                case SessionSwitchReason.ConsoleConnect:
                    break;
                case SessionSwitchReason.ConsoleDisconnect:
                    break;
                case SessionSwitchReason.RemoteConnect:
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    break;
                case SessionSwitchReason.SessionLogon:
                    break;
                case SessionSwitchReason.SessionLogoff:
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };

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
