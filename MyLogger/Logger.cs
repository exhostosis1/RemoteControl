using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace MyLogger
{
    public class Logger : ILogger
    {
        #region LogData

        private class LogData
        {
            public AbstractLogWriter Writer { get; set; }
            public string Message { get; set; }
            public Type Type { get; set; }
            public DateTime Time { get; set; }

            public override string ToString() => $"{Time:R} - {Type.FullName} - {Message}";
        }

        #endregion

        #region Statics

        private static readonly Timer Timer = new Timer
        {
            Interval = 30_000,
            AutoReset = true,
            Enabled = true
        };

        private static ConcurrentQueue<LogData> _data = new ConcurrentQueue<LogData>();

        static Logger()
        {
            Timer.Elapsed += DoJob;
            Timer.Disposed += DoJob;
        }

        private static void DoJob(object sender, EventArgs args)
        {
            if (_data.Count == 0) return;

            var groupedWriters = _data.GroupBy(x => x.Writer).ToList();
            _data = new ConcurrentQueue<LogData>();

            foreach (var group in groupedWriters)
            {
                group.Key.WriteDataAsync(group.Select(x => x.ToString()));
            }
        }

        public static void Flush(object sender, EventArgs args)
        {
            DoJob(null, null);
        }

        public static ILogger GetFileLogger(string fileName, Type type)
        {
            return new Logger(FileWriter.GetLogger(fileName), type);
        }

        #endregion

        private readonly AbstractLogWriter _writer;

        
        private readonly Type _type;

        public void Log(string message) => _data.Enqueue(new LogData
            {Message = message, Type = _type, Writer = _writer, Time = DateTime.Now});

       

        private Logger(AbstractLogWriter writer, Type type)
        {
            _writer = writer;
            _type = type;
        }
    }
}
