using System;
using System.Collections.Generic;

namespace MyLogger
{
    public abstract class AbstractLogProvider : IDisposable
    {
        protected readonly List<string> Data = new List<string>();
        private readonly Type _type;

        public void Log(string message) => Data.Add($"{DateTime.Now:G} - {_type.Name} - {message}");

        protected AbstractLogProvider(Type type)
        {
            _type = type;
            LogScheduler.RegisterProvider(this);
        }

        internal abstract void WriteData();

        public void Dispose()
        {
            WriteData();
            Data.Clear();
            LogScheduler.UnregisterProvider(this);
        }
    }
}
