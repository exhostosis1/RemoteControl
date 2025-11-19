using MainApp.ControlProviders.Interfaces;
using MainApp.Workers.Results;
using Microsoft.Extensions.Logging;

namespace MainApp.Workers.ApiControllers;

internal class DisplayController(IDisplayControl provider, ILogger logger) : BaseApiController
{
    public IActionResult Darken()
    {
        logger.LogInformation("Turning off display");

        provider.DisplayOff();

        return Ok();
    }
}