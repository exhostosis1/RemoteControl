using AppHost;

namespace MainUI;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        var appBuilder = new AppHostBuilder();

        var app = Task.Run(() => appBuilder.Build()).Result;

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.Run(new MainForm(app));
    }
}