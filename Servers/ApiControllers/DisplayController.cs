using Microsoft.Extensions.Logging;
using Servers.Results;
using Shared.ControlProviders.Provider;

namespace Servers.ApiControllers;

public class DisplayController(IDisplayControlProvider provider, ILogger logger) : BaseApiController
{
    public IActionResult Darken()
    {
        logger.LogInformation("Turning off display");

        provider.DisplayOff();

        return Ok();
    }
}