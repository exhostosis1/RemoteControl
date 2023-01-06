using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Windows;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var container = new RemoteControlContainer();

        var indexes = new List<int>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    container.Logger.LogInfo("Stopping processords due to logout");

                    indexes.Clear();

                    for (var i = 0; i < RemoteControl.Main.ControlProcessors.Count; i++)
                    {
                        if (RemoteControl.Main.ControlProcessors[i].Working)
                        {
                            indexes.Add(i);
                            RemoteControl.Main.ControlProcessors[i].Stop();
                        }
                    }

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    container.Logger.LogInfo("Resoring processors");

                    foreach (var index in indexes)
                    {
                        RemoteControl.Main.ControlProcessors[index].Start();
                    }
                    break;
                }
                default:
                    break;
            }
        };

        try
        {
            RemoteControl.Main.Run(container);
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }
    }
}
