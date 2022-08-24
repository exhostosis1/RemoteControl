using Logging;
using RemoteControlWinForms;
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
                var logger = new FileLogger("error.log");
                var uri = container.Config.GetConfig().UriConfig.Uri;

                container.Server.Start(uri);

                var form = new ConfigForm(new ViewModel(uri.ToString(), container.Server.IsListening, container.Autostart.CheckAutostart()), logger);
                
                form.StartEvent = () =>
                {
                    try
                    {
                        container.Server.Start(uri);
                    }
                    catch (Exception exception)
                    {
                        logger.Log(exception.Message);
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

                RemoteControlWinForms.Program.Main(form);

                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(@"OS is not supported");
            }
        }
    }
}
