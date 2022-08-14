using RemoteControl.App;
using RemoteControl.App.Control.Wrappers;
using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Listeners;
using RemoteControl.App.Web.Middleware;
using RemoteControl.Autostart;
using RemoteControl.Config;

namespace RemoteControl
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            var logger = new FileLogger(AppContext.BaseDirectory + "error.log");

            var audio = new AudioSwitchWrapper();
            var input = new WindowsInputLibWrapper();
            var display = new User32Wrapper();

            var uiListener = new GenericListener();
            var apiListener = new GenericListener();
            var fileController = new FileMiddleware();

            var controllers = new BaseController[]
            {
                new AudioController(audio),
                new KeyboardController(input),
                new MouseController(input),
                new DisplayController(display)
            };

            var apiController = new ApiMiddlewareV1(controllers);

            var app = new RemoteControlApp(uiListener, apiListener, fileController, apiController);
            var config = new LocalFileConfigService(logger);
            var autostart = new WinAutostartService();

            var form = new ConfigForm(app, config, autostart, logger);

            Application.Run(form);
        }
    }
}