using Shared.Logging.Interfaces;

namespace ControlProviders.Abstract;

public abstract class BaseProvider
{
    protected readonly ILogger Logger;

    protected BaseProvider(ILogger logger)
    {
        Logger = logger;
    }
}