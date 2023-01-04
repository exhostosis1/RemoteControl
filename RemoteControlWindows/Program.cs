using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var indexes = new List<int>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    indexes.Clear();

                    for (var i = 0; i < RemoteControlMain.Main.ControlProcessors.Count; i++)
                    {
                        if (RemoteControlMain.Main.ControlProcessors[i].Working)
                        {
                            indexes.Add(i);
                            RemoteControlMain.Main.ControlProcessors[i].Stop();
                        }
                    }

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    foreach (var index in indexes)
                    { 
                        RemoteControlMain.Main.ControlProcessors[index].Start();
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
