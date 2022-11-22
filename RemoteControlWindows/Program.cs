using Microsoft.Win32;

namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();

        Uri? listeningUri = null;

        SystemEvents.SessionSwitch += (_, args) =>
        {
            if(args.Reason == SessionSwitchReason.SessionLock)
            {
                listeningUri = container.Server.IsListening ? container.Server.GetListeningUri() : null;
                if (listeningUri != null)
                    container.Server.Stop();
            }

            if(args.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (listeningUri != null)
                    container.Server.Start(listeningUri);
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
