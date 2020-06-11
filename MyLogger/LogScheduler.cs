using System;
using System.Collections.Concurrent;
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
            Timer.Interval = interval;
            Timer.Start();
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        static LogScheduler()
        {
            Timer.Elapsed += DoJob;
            Timer.Disposed += DoJob;
        }

        private static void DoJob(object sender, EventArgs args)
        {
            if (Logger.Data.Count == 0) return;

            var groupedWriters = Logger.Data.GroupBy(x => x.Writer).ToList();
            Logger.Data = new ConcurrentQueue<LogData>();

            foreach (var group in groupedWriters)
            {
                group.Key.WriteDataAsync(group.Select(x => x.ToString()));
            }
        }

        public static void Flush()
        {
            DoJob(null, null);
        }
    }
}
