using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Controllers;

public class DisplayController: BaseController
{
    private readonly IDisplayControlProvider _display;

    public DisplayController(IDisplayControlProvider display, ILogger logger) : base(logger)
    {
        _display = display;
    }

    public IActionResult Darken(string? _)
    {
        _display.Darken();

        return Ok();
    }
}