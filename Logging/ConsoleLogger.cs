using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging
{
    public class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger(LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) : base(level, formatter)
        {}

        protected override void ProcessMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
