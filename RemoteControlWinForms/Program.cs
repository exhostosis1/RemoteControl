using Shared;
using Shared.Logging.Interfaces;

namespace RemoteControlWinForms
{
    public static class Program
    {
        private static IContainer _container;
        private static ILogger _logger;

        public static void Inject(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        //[STAThread]
        public static void Main()
        {
            if (_container == null || _logger == null)
                throw new Exception("Should call Inject(IContainer, ILogger) first");

            ApplicationConfiguration.Initialize();

            var form = new ConfigForm(_container.Server, _container.Config, _container.Autostart, _logger);

            Application.Run(form);
        }
    }
}