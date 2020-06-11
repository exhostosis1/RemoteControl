using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace MyLogger
{
    internal static class LogScheduler
    {
        private static readonly Timer Timer = new Timer
        {
            Enabled = false,
            AutoReset = true

        };

        public static void Start(int interval)
        {
            if (Timer.Enabled)
                Timer.Stop();

            Timer.Interval = interval;
            Timer.Start();
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        static LogScheduler()
        {
            Timer.Elapsed += HandleEvent;
            Timer.Disposed += HandleEvent;
        }

        private static void HandleEvent(object sender, EventArgs args) => DoJob();

        private static void DoJob()
        {
            if (Logger.Data.Count == 0) return;

            List<IGrouping<ILogWriter, LogData>> groupedWriters;

            lock (Logger.Data)
            {
                groupedWriters = Logger.Data.GroupBy(x => x.Writer).ToList();
                Logger.Data = new ConcurrentQueue<LogData>();
            }

            foreach (var group in groupedWriters)
            {
                group.Key.WriteDataAsync(group.Select(x => x.ToString()));
            }
        }

        public static void Flush()
        {
            DoJob();
        }
    }
}
