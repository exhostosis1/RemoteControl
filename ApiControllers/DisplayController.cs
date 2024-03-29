using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController(IDisplayControlProvider provider, ILogger<DisplayController> logger) : BaseApiController
{
    public IActionResult Darken(string? _)
    {
        logger.LogInfo("Turning off display");

        provider.DisplayOff();

        return Ok();
    }
}