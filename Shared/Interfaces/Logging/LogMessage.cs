using Shared.Enums;

namespace Shared.Interfaces.Logging
{
    public record LogMessage(LoggingLevel Level, DateTime Date, string Message);
}
