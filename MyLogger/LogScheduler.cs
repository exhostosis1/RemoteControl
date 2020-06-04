using System;
using System.Timers;
using System.Collections.Generic;

namespace MyLogger
{
    public static class LogScheduler
    {
        private static readonly List<AbstractLogProvider> Providers = new List<AbstractLogProvider>();

        private static readonly Timer Timer = new Timer
        {
            Interval = 60_000,
            AutoReset = true,
            Enabled = false
        };

        private static void DoJob(object sender, EventArgs args)
        {
            Providers.ForEach(x => x.WriteData());
        }

        static LogScheduler()
        {
            Timer.Elapsed += DoJob;
            Timer.Disposed += DoJob;
        }

        public static void RegisterProvider(AbstractLogProvider provider)
        {
            Providers.Add(provider);
            Timer.Start();
        }

        public static void UnregisterProvider(AbstractLogProvider provider)
        {
            Providers.Remove(provider);
            if(Providers.Count == 0) Timer.Stop();
        }
    }
}
