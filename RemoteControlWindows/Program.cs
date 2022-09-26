namespace RemoteControlWindows;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();

        RemoteControlMain.Main.Run(container);
    }
}
