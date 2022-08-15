using Shared;
using Shared.Interfaces.Logging;

namespace Logging.Formatters
{
    public class DefaultMessageFormatter: IMessageFormatter
    {
        public string Format(LogMessage message) => $@"{message.Level} {message.Date:G} {message.Message}";
    }
}
