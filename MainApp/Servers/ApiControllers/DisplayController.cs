using MainApp.ControlProviders.Interfaces;
using MainApp.Servers.Results;
using Microsoft.Extensions.Logging;

namespace MainApp.Servers.ApiControllers;

internal class DisplayController(IDisplayControl provider, ILogger logger) : BaseApiController
{
    public IActionResult Darken()
    {
        logger.LogInformation("Turning off display");

        provider.DisplayOff();

        return Ok();
    }
}