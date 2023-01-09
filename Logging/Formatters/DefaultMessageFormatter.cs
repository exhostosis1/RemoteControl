using Shared.Logging;
using Shared.Logging.Interfaces;

namespace Logging.Formatters;

public class DefaultMessageFormatter: IMessageFormatter
{
    public string Format(LogMessage message) => $@"{message.Date:G} {message.Level} {message.CallerType} {message.Message}";
}