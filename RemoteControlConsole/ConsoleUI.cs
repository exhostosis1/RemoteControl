using Shared;
using Shared.Logging.Interfaces;

namespace RemoteControlConsole
{
    public class ConsoleUI: IUserInterface
    {
        private ViewModel? _model;
        private readonly ILogger _logger;

        public ConsoleUI(ViewModel model, ILogger logger)
        {
            _model = model;
            _logger = logger;
        }

        void Main()
        {
            try
            {
                _model = StartEvent?.Invoke();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            
            DisplayInfo();

            while (true)
            {
                var key = Console.ReadLine();

                if (key == "x") return;

                switch (key)
                {
                    case "s":
                        _model = _model?.IsListening ?? true ? StopEvent?.Invoke() : StartEvent?.Invoke();
                        break;
                    case "a":
                        _model = AutostartEvent?.Invoke();
                        break;
                    default:
                        continue;
                }

                DisplayInfo();
            }
        }

        private void DisplayInfo()
        {
            Console.WriteLine(_model?.IsListening ?? false ? $"Server listening on {_model.Uri}" : "Server stopped");
            Console.WriteLine($"Autostart {(_model?.Autostart ?? false ? "enabled" : "disabled")}");
            Console.WriteLine();

            Console.Write($"{(_model?.IsListening ?? false ? "[s]top" : "[s]tart")}, toggle [a]utostart, e[x]it:");
        }

        public Func<ViewModel>? StartEvent { get; set; }
        public Func<ViewModel>? StopEvent { get; set; }
        public Func<ViewModel>? AutostartEvent { get; set; }
        public void ShowUI()
        {
            Main();
        }
    }
}