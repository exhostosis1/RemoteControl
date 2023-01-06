using Shared.ApiControllers;
using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController: BaseApiController
{
    private readonly IDisplayControlProvider _display;

    public DisplayController(IDisplayControlProvider display, ILogger logger) : base(logger)
    {
        _display = display;
    }

    public IActionResult Darken(string? _)
    {
        Logger.LogInfo("Turning off display");

        _display.Darken();

        return Ok();
    }
}