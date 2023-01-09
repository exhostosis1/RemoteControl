using System;
using Shared.Enums;

namespace Shared.Logging;

public class LogMessage
{
    public Type CallerType { get; set; }
    public LoggingLevel Level { get; set; }
    public DateTime Date { get; set; }
    public string Message { get; set; }

    public LogMessage(Type callerType, LoggingLevel level, DateTime date, string message)
    {
        CallerType = callerType;
        Level = level;
        Date = date;
        Message = message;
    }
}