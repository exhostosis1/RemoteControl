using Shared.Enums;

namespace Shared.Logging
{
    public record LogMessage(LoggingLevel Level, DateTime Date, string Message);
}
