using System.Collections.Concurrent;

namespace RemoteControl
{
    internal static class Logger
    {
        private static readonly BlockingCollection<string> Messages = new ();

        static Logger()
        {
            new TaskFactory().StartNew(WriteMessages, TaskCreationOptions.LongRunning);
        }

        public static void Log(string message)
        {
            Messages.Add(message);
        }

        private static void WriteMessages()
        {
            foreach (var msg in Messages.GetConsumingEnumerable())
            {
                File.AppendAllText(AppContext.BaseDirectory + "error.log", $"{DateTime.Now:G} {msg}\n");
            }
        }
    }
}
