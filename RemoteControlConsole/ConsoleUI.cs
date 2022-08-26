using Shared;

namespace RemoteControlConsole
{
    // ReSharper disable once InconsistentNaming
    public class ConsoleUI: IUserInterface
    {
        public event EmptyEventHandler? StartEvent;
        public event EmptyEventHandler? StopEvent;
        public event BoolEventHandler? AutostartChangeEvent;
        public event EmptyEventHandler? CloseEvent;
        
        public Uri? Uri { get; set; }
        public bool IsListening { get; set; }
        public bool IsAutostart { get; set; }

        // ReSharper disable once InconsistentNaming
        public void RunUI()
        {
            DisplayInfo();

            while (true)
            {
                var key = Console.ReadLine();

                if (key == "x")
                {
                    CloseEvent?.Invoke();
                    return;
                }

                switch (key)
                {
                    case "s":
                        if(IsListening)
                            StopEvent?.Invoke();
                        else
                            StartEvent?.Invoke();
                        break;
                    case "a":
                        AutostartChangeEvent?.Invoke(!IsAutostart);
                        break;
                    default:
                        continue;
                }

                DisplayInfo();
            }
        }

        private void DisplayInfo()
        {
            Console.WriteLine(IsListening ? $"Server listening on {Uri}" : "Server stopped");
            Console.WriteLine($"Autostart {(IsAutostart ? "enabled" : "disabled")}");
            Console.WriteLine();

            Console.Write($"{(IsListening ? "[s]top" : "[s]tart")}, [a]utostart, e[x]it:");
        }
    }
}