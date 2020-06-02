using System;
using System.Collections.Concurrent;
using System.IO;
using System.Timers;

namespace MyLogger
{
    public class Logger
    {
        private const string FileName = "log.txt";
        private readonly string _typeName;
        private readonly Timer _timer;
        private ConcurrentQueue<string> _data = new ConcurrentQueue<string>();
        private const long MaxSize = 100 * 1024 * 1024;
        
        public Logger(Type type)
        {
            var file = new FileInfo(FileName);
            if (file.Exists && file.Length >= MaxSize)
            {
                file.Delete();
                file.Create();
            }

            _typeName = type.Name;
            _timer = new Timer
            {
                Interval = 60_000, 
                Enabled = true,
                AutoReset = true
            };
            _timer.Elapsed += WriteData;
            _timer.Disposed += WriteData;
            _timer.Start();
        }

        private void WriteData(object sender, EventArgs state)
        {
            if (_data.Count == 0) return;

            File.AppendAllLines(FileName, _data);
            _data = new ConcurrentQueue<string>();
            GC.Collect();
        }

        public void Log(string input)
        {
            var loggingText = $"{DateTime.Now.ToLongTimeString()} - {_typeName} - {input}";
            _data.Enqueue(loggingText);
        }

        ~Logger()
        {
            _timer.Dispose();
            _data = null;
        }
    }
}
