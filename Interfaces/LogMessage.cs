using Shared.Enums;

namespace Shared
{
    public record LogMessage(LoggingLevel Level, DateTime Date, string Message);
}
