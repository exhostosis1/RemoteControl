using System.Collections.Concurrent;
using RemoteControl.App.Interfaces;

namespace RemoteControl
{
    public class FileLogger: ILogger
    {
        private readonly BlockingCollection<string> _messages = new ();
        private readonly string _path;

        public FileLogger(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !Path.IsPathFullyQualified(filePath))
                throw new ArgumentException(nameof(filePath));

            if (!File.Exists(filePath))
                File.Create(filePath);

            _path = filePath;

            new TaskFactory().StartNew(WriteMessages, TaskCreationOptions.LongRunning);
        }

        public void Log(string message)
        {
            _messages.Add(message);
        }

        private void WriteMessages()
        {
            foreach (var msg in _messages.GetConsumingEnumerable())
            {
                File.AppendAllText(_path, $"{DateTime.Now:G} {msg}\n");
            }
        }
    }
}
