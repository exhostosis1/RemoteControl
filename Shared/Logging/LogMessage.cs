using System;
using Shared.Enums;

namespace Shared.Logging
{
    public class LogMessage
    {
        public LoggingLevel Level { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        public LogMessage(LoggingLevel level, DateTime date, string message)
        {
            Level = level;
            Date = date;
            Message = message;
        }
    }
}
