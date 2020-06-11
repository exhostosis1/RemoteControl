using System;
using System.Collections.Concurrent;

namespace MyLogger
{
    public class Logger : ILogger
    {
        internal static ConcurrentQueue<LogData> Data = new ConcurrentQueue<LogData>();
        
        private readonly ILogWriter _writer;

        private readonly Type _type;

        public Logger(ILogWriter writer, Type type)
        {
            _writer = writer;
            _type = type;

            LogScheduler.Start(30_000);
        }

        public void Log(string message) => Data.Enqueue(new LogData
            {Message = message, Type = _type, Writer = _writer, Time = DateTime.UtcNow});


        public static ILogger GetFileLogger(string fileName, Type type)
        {
            return new Logger(FileWriter.GetFileWriter(fileName), type);
        }

        public static void Flush(object sender, EventArgs args)
        {
            LogScheduler.Flush();
        }
    }
}
