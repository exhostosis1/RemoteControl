using ControlProviders.Interfaces;
using Microsoft.Extensions.Logging;
using Servers.Results;

namespace Servers.ApiControllers;

public class DisplayController(IDisplayControl provider, ILogger logger) : BaseApiController
{
    public IActionResult Darken()
    {
        logger.LogInformation("Turning off display");

        provider.DisplayOff();

        return Ok();
    }
}