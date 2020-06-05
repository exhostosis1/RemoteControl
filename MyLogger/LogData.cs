using System;

namespace MyLogger
{
    internal class LogData
    {
        public ILogWriter Writer { get; set; }
        public string Message { get; set; }
        public Type Type { get; set; }
        public DateTime Time { get; set; }

        public override string ToString() => $"{Time:R} - {Type.FullName} - {Message}";
    }
}
