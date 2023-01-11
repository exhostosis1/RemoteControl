using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController: BaseApiController
{
    private readonly IDisplayControlProvider _display;
    private readonly ILogger<DisplayController> _logger;

    public DisplayController(IDisplayControlProvider display, ILogger<DisplayController> logger) : base(logger)
    {
        _logger = logger;
        _display = display;
    }

    public IActionResult Darken(string? _)
    {
        _logger.LogInfo("Turning off display");

        _display.Darken();

        return Ok();
    }
}