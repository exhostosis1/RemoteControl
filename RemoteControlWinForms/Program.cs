using Autostart;
using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Web.Controllers;
using RemoteControlApp.Web.Middleware;
using Shared.Enums;

namespace RemoteControlWinForms
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

            var fileLogger = new FileLogger(AppContext.BaseDirectory + "error.log");
            var consoleLogger = new ConsoleLogger(LoggingLevel.Info);

            var audio = new AudioSwitchWrapper();
            var input = new WindowsInputLibWrapper();
            var display = new User32Wrapper();

            var uiListener = new GenericListener();
            var apiListener = new GenericListener();
            var fileController = new FileMiddleware();

            var controllers = new BaseController[]
            {
                new AudioController(audio, consoleLogger),
                new KeyboardController(input, consoleLogger),
                new MouseController(input, consoleLogger),
                new DisplayController(display, consoleLogger)
            };

            var apiController = new ApiMiddlewareV1(controllers);

            var app = new RemoteControl(uiListener, apiListener, fileController, apiController);
            var config = new LocalFileConfigService(fileLogger);
            var autostart = new WinAutostartService();

            var form = new ConfigForm(app, config, autostart, fileLogger);

            Application.Run(form);
        }
    }
}