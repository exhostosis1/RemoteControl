using Shared.Config;
using Shared.Interfaces;
using Shared.Interfaces.Logging;

namespace RemoteControlWinForms
{
    public static class Program
    {
        private static IRemoteControlApp _app;
        private static IConfigService _config;
        private static IAutostartService _autostart;
        private static ILogger _logger;

        public static void Inject(IRemoteControlApp app, IConfigService config, IAutostartService autostart,
            ILogger logger)
        {
            _app = app;
            _config = config;
            _autostart = autostart;
            _logger = logger;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();

            var form = new ConfigForm(_app, _config, _autostart, _logger);

            Application.Run(form);
        }
    }
}