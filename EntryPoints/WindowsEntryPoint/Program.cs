using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace WindowsEntryPoint;

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

                    for (var i = 0; i < RemoteControlMain.Program.ControlProcessors.Count; i++)
                    {
                        if (RemoteControlMain.Program.ControlProcessors[i].Working)
                        {
                            indexes.Add(i);
                            RemoteControlMain.Program.ControlProcessors[i].Stop();
                        }
                    }

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    container.Logger.LogInfo("Resoring processors");

                    foreach (var index in indexes)
                    {
                        RemoteControlMain.Program.ControlProcessors[index].Start();
                    }
                    break;
                }
                default:
                    break;
            }
        };

        try
        {
            RemoteControlMain.Program.Run(container);
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }
    }
}
