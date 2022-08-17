using Shared.Interfaces.Logging;

namespace Control.Wrappers
{
    public abstract class BaseWrapper
    {
        protected readonly ILogger Logger;

        protected BaseWrapper(ILogger logger)
        {
            Logger = logger;
        }
    }
}
