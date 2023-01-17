using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController: BaseApiController
{
    private readonly IControlProvider _provider;
    private readonly ILogger<DisplayController> _logger;

    public DisplayController(IControlProvider provider, ILogger<DisplayController> logger) : base(logger)
    {
        _logger = logger;
        _provider = provider;
    }

    public IActionResult Darken(string? _)
    {
        _logger.LogInfo("Turning off display");

        _provider.DisplayOff();

        return Ok();
    }
}