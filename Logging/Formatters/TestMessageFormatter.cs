using Shared.Logging;
using Shared.Logging.Interfaces;

namespace Logging.Formatters;

public class TestMessageFormatter : IMessageFormatter
{
    public string Format(LogMessage message) => $@"{message.Level} {message.CallerType} {message.Message}";
}