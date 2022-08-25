using RemoteControlWinForms;
using Shared;
using System.Runtime.InteropServices;

namespace RemoteControl
{
    public static class Program
    {
        public static void Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var container = new RemoteControlContainer();
                var uri = container.Config.GetConfig().UriConfig.Uri;

                container.Server.Start(uri);

                var form = new RemoteControlConsole.ConsoleUI(new ViewModel(uri.ToString(), container.Server.IsListening, container.Autostart.CheckAutostart()), container.DefaultLogger);
                
                form.StartEvent = () =>
                {
                    try
                    {
                        container.Server.Start(uri);
                    }
                    catch (Exception exception)
                    {
                        container.DefaultLogger.Log(exception.Message);
                        throw;
                    }

                    return new ViewModel(container.Server.GetListeningUri(), container.Server.IsListening,
                        container.Autostart.CheckAutostart());
                };

                form.StopEvent = () =>
                {
                    container.Server.Stop();

                    return new ViewModel(container.Server.GetListeningUri(), container.Server.IsListening,
                        container.Autostart.CheckAutostart());
                };

                form.AutostartEvent = () =>
                {
                    var autostart = container.Autostart.CheckAutostart();
                    container.Autostart.SetAutostart(!autostart);

                    return new ViewModel(container.Server.GetListeningUri(), container.Server.IsListening,
                        container.Autostart.CheckAutostart());
                };

                form.ShowUI();
            }
            else
            {
                Console.WriteLine(@"OS is not supported");
            }
        }
    }
}
