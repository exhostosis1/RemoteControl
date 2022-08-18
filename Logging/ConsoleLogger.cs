using Logging.Abstract;
using Shared.Enums;
using Shared.Interfaces.Logging;

namespace Logging
{
    public class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger(LoggingLevel level, IMessageFormatter? formatter) : base(level, formatter)
        {}

        public ConsoleLogger(LoggingLevel level) : base(level, null)
        { }

        public ConsoleLogger(IMessageFormatter? formatter) : base(LoggingLevel.Info, formatter)
        { }

        public ConsoleLogger() : base(LoggingLevel.Info, null)
        { }

        protected override void ProcessMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
